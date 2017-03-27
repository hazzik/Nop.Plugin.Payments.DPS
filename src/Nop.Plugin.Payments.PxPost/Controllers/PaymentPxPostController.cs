using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hazzik.Nop.Plugin.Payments.PxPost.Models;
using Hazzik.Nop.Plugin.Payments.PxPost.Validators;
using Nop.Core;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Controllers
{
    public class PaymentPxPostController : BasePaymentController
    {
        readonly IWorkContext _workContext;
        readonly IStoreService _storeService;
        readonly ISettingService _settingService;
        readonly ILocalizationService _localizationService;

        public PaymentPxPostController(IWorkContext workContext,
                                       IStoreService storeService,
                                       ISettingService settingService,
                                       ILocalizationService localizationService)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<PxPostPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                TransactModeId = Convert.ToInt32(settings.TransactMode),
                AdditionalFee = settings.AdditionalFee,
                AdditionalFeePercentage = settings.AdditionalFeePercentage,
                TransactModeValues = settings.TransactMode.ToSelectList(),
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.TransactModeId_OverrideForStore = _settingService.SettingExists(settings, s => s.TransactMode, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(settings, s => s.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(settings, s => s.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Hazzik.Nop.Plugin.Payments.PxPost/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<PxPostPaymentSettings>(storeScope);

            settings.TransactMode = (TransactMode) model.TransactModeId;
            settings.AdditionalFee = model.AdditionalFee;
            settings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            _settingService.SaveSettingOverridablePerStore(settings, s => s.TransactMode, model.TransactModeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, s => s.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, s => s.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();

            model.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Visa",
                Value = "Visa",
            });
            model.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Master card",
                Value = "MasterCard",
            });
            model.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Discover",
                Value = "Discover",
            });
            model.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Amex",
                Value = "Amex",
            });

            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                model.ExpireYears.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
            }

            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i : i.ToString();
                model.ExpireMonths.Add(new SelectListItem
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }

            var form = Request.Form;
            model.CardholderName = form["CardholderName"];
            model.CardNumber = form["CardNumber"];
            model.CardCode = form["CardCode"];
            var selectedCcType = model.CreditCardTypes.FirstOrDefault(x => x.Value.Equals(form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedCcType != null)
                selectedCcType.Selected = true;
            var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;

            return View("~/Plugins/Hazzik.Nop.Plugin.Payments.PxPost/Views/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            var validator = new PaymentInfoValidator(_localizationService);
            var model = new PaymentInfoModel
            {
                CardholderName = form["CardholderName"],
                CardNumber = form["CardNumber"],
                CardCode = form["CardCode"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"]
            };
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                foreach (var error in validationResult.Errors)
                    warnings.Add(error.ErrorMessage);
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            return new ProcessPaymentRequest
            {
                CreditCardType = form["CreditCardType"],
                CreditCardName = form["CardholderName"],
                CreditCardNumber = form["CardNumber"],
                CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
                CreditCardExpireYear = int.Parse(form["ExpireYear"]),
                CreditCardCvv2 = form["CardCode"],
                CustomValues =
                {
                    ["CurrencyCode"] = _workContext.WorkingCurrency.CurrencyCode
                }
            };
        }
    }
}