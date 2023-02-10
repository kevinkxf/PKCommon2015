using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
namespace PKCommon
{
    public struct Global
    {
        //System Setup Function
        public const string SYSTEM_SETUP_FUNCTION_Password = "7675464";
        public const string TECHNICIAN_ID = "6048086721";
        public const string TECHNICIAN_Password = "7675464";

        public const string USER_DEFAULT_Password = "0000";
        public const string RESET_PASSWORD_Successful = "Reset password successful!";
        public const string RESET_PASSWORD_Failed = "Reset password failed!";

        public const string HST_TAX_ID = "TAX001";

        //Log
        public const string USER_ACTIVITY_Login = "Login";
        public const string USER_ACTIVITY_LoginFailed = "Login Failed";
        public const string USER_ACTIVITY_SignOut = "Sign Out";
        public const string USER_ACTIVITY_Exit = "Exit";
        public const string USER_ACTIVITY_ExitFailed = "Exit Failed";
        public const string USER_ACTIVITY_Sell = "Sell";
        public const string USER_ACTIVITY_Return = "Return";

        //Feature
        public const string SYSTEM_FEATURE_Payment = "Feature_Payment";
        public const string SYSTEM_SUB_FEATURE_Payment_CancelPayment = "Feature_Payment_CancelPayment";
        public const string SYSTEM_SUB_FEATURE_Payment_Cash = "Feature_Payment_Cash";
        public const string SYSTEM_SUB_FEATURE_Payment_Debit = "Feature_Payment_Debit";
        public const string SYSTEM_SUB_FEATURE_Payment_Visa = "Feature_Payment_Visa";
        public const string SYSTEM_SUB_FEATURE_Payment_MasterCard = "Feature_Payment_MasterCard";
        public const string SYSTEM_SUB_FEATURE_Payment_AE = "Feature_Payment_AE";
        public const string SYSTEM_SUB_FEATURE_Payment_StoreCredit = "Feature_Payment_StoreCredit";
        public const string SYSTEM_SUB_FEATURE_Payment_JCB = "Feature_Payment_JCB";
        public const string SYSTEM_SUB_FEATURE_Payment_USD = "Feature_Payment_USD";
        //--
        public const string SYSTEM_FEATURE_Discount = "Feature_Discount";
        public const string SYSTEM_SUB_FEATURE_Discount_ItemDiscount = "Feature_Discount_ItemDiscount";
        public const string SYSTEM_SUB_FEATURE_Discount_PriceOverride = "Feature_Discount_PriceOverride";
        public const string SYSTEM_SUB_FEATURE_Discount_SubTotalDiscount = "Feature_Discount_SubTotalDiscount";
        public const string SYSTEM_SUB_FEATURE_Discount_HSTIncluded = "Feature_Discount_HSTIncluded";
        //--
        public const string SYSTEM_FEATURE_Management = "Feature_Management";
        public const string SYSTEM_SUB_FEATURE_Management_Report = "Feature_Management_Report";
        public const string SYSTEM_SUB_FEATURE_Management_RePrintReceipt = "Feature_Management_RePrintReceipt";
        public const string SYSTEM_SUB_FEATURE_Management_MultiItemBarcodePrint = "Feature_Management_MultiItemBarcodePrint";
        public const string SYSTEM_SUB_FEATURE_Management_UserManagement = "Feature_Management_UserManagement";
        public const string SYSTEM_SUB_FEATURE_Management_OpenCashDrawer = "Feature_Management_OpenCashDrawer";
        public const string SYSTEM_SUB_FEATURE_Management_UpdateUSDRate = "Feature_Management_UpdateUSDRate";
        public const string SYSTEM_SUB_FEATURE_Management_ConsignorManagement = "Feature_Management_ConsignorManagement";
        public const string SYSTEM_SUB_FEATURE_Management_CustomerManagement = "Feature_Management_CustomerManagement";
        public const string SYSTEM_SUB_FEATURE_Management_DeptCateManagement = "Feature_Management_DeptCateManagement";
        public const string SYSTEM_SUB_FEATURE_Management_InventoryManagement = "Feature_Management_InventoryManagement";

        public const string SYSTEM_FEATURE_UPDATE_Successful = "Update successful!";



        //Consignor
        public const string SYSTEM_SETTING_VALUEDATATYPE_String = "string";
        public const string SYSTEM_SETTING_VALUEDATATYPE_Decimal = "decimal";
        public const string SYSTEM_SETTING_STATUS = "Available";
        public const string SYSTEM_SETTING_DESCRIPTION_UserAdd = "USER";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlCity = "ddlCity";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlState = "ddlState";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlNotSoldResult = "ddlNotSoldResult";
        public const string SYSTEM_SETTING_FUNCTIONNAME_nudFinalizedItemDay = "nudFinalizedItemDay";
        public const string SYSTEM_SETTING_FUNCTIONNAME_nudSharePercent = "nudSharePercent";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlPurchaseMethod = "ddlPurchaseMethod";
        public const string SYSTEM_SETTING_FUNCTIONNAME_dtpReportFromTime = "dtpReportFromTime";
        public const string SYSTEM_SETTING_FUNCTIONNAME_dtpReportToTime = "dtpReportToTime";
        public const string SYSTEM_SETTING_FUNCTIONNAME_nudUSDRate = "nudUSDRate";
        public const string SYSTEM_SETTING_FUNCTIONNAME_HSTTaxIncludeRate = "HSTTaxIncludeRate";

        public const string SETTING_RECEIPT_SALE_COPYS = "ReceiptSellCopys";
        public const string SETTING_RECEIPT_RETURN_COPYS = "ReceiptReturnCopys";

        public const string VALIDATION_ERROR_MESSAGE_Warning = "Warning!";
        public const string VALIDATION_ERROR_MESSAGE_Oops = "Oops!";
        public const string VALIDATION_ERROR_MESSAGE_Information = "Information";
        public const string VALIDATION_ERROR_MESSAGE_FailureToSave = "Failure to Save! ";
        public const string VALIDATION_ERROR_MESSAGE_DataSaved = "Data successfully Saved! ";
        public const string VALIDATION_ERROR_MESSAGE_FailureToUpdate = "Failure to Update! ";
        public const string VALIDATION_ERROR_MESSAGE_RequireField = " is required.";
        public const string VALIDATION_ERROR_MESSAGE_CanNotBeDelete = " is required.";
        public const string VALIDATION_ERROR_MESSAGE_ThisFieldValue = "This field Value";
        public const string VALIDATION_ERROR_MESSAGE_DecimalDollarsGreaterThanZero = " must be in numeric , greater than zero and rounded to the nearest cent.";
        public const string VALIDATION_ERROR_MESSAGE_GreaterThanZero = "The number must be greater than zero.";
        public const string VALIDATION_ERROR_MESSAGE_DecimalQty = " must be in numeric or rounded to the second decimal place.";
        public const string VALIDATION_ERROR_MESSAGE_TheInputTooLong = "The input text is too long.";
        public const string VALIDATION_ERROR_MESSAGE_LoginIDAvailable = "Someone already has that Login ID. Try another?";
        public const string VALIDATION_ERROR_MESSAGE_DateType = "must be in date type";
        public const string VALIDATION_ERROR_MESSAGE_OnlyOneTaxfree = "Only one Tax Free Promotion is allowed.Jumped to the original one already.";

        public static string DATABASE_ACCESS_ERROR_MESSAGE = string.Empty;

        //Checkout Infomation
        public const string CHECK_OUT_INFO_TEXT_Tendered = "Tendered:";
        public const string CHECK_OUT_INFO_TEXT_Payout = "Payout:";
        public const string CHECK_OUT_INFO_TEXT_Change = "Change:";
        public const string CHECK_OUT_INFO_TEXT_Due = "Due:";
        public const string CHECK_OUT_INFO_TEXT_RefundDue = "Refund Due:";
        public const string CHECK_OUT_INFO_TEXT_Overpayment = "Overpayment:";


        public const string Consignor_Search_Criteria_Name = "Name";
        public const string Consignor_Search_Criteria_IDNumber = "ID#";
        public const string Consignor_Search_Criteria_Phone = "Phone";
        public const string Consignor_Number_Deleted_Status = "Deleted";

        //Product Description
        public const string PRODUCT_STATUS_Available = "Available";
        public const string PRODUCT_STATUS_Pending = "Pending";
        public const string PRODUCT_STATUS_Cancel = "Cancel";
        public const string PRODUCT_STATUS_Deleted = "Deleted";

        public const string PRODUCT_DESCRIPTION_CONDITION_PreOwned = "Pre-owned";
        public const string PRODUCT_DESCRIPTION_CONDITION_BrandNew = "Brand New";

        public const string PRODUCT_DESCRIPTION_STOCKSTATUS_Available = "Available";
        public const string PRODUCT_DESCRIPTION_STOCKSTATUS_Sold = "Sold";
        public const string PRODUCT_DESCRIPTION_STOCKSTATUS_Deleted = "Deleted";
        public const string PRODUCT_DESCRIPTION_STOCKSTATUS_InTransaction = "InTransaction";
        public const string PRODUCT_DESCRIPTION_STOCKSTATUS_Hold = "HOLD";

        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlUnitName = "ddlUnitName";

        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlMadeIn = "ddlMadeIn";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlCondition = "ddlCondition";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlSubCondition = "ddlSubCondition";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlSkin = "ddlSkin";
        public const string SYSTEM_SETTING_FUNCTIONNAME_ddlColor = "ddlColor";

        public const string PRODUCT_DESCRIPTION_SEARCH_FIELD_2 = "ddl_PD_Search_2";
        public const string PRODUCT_DESCRIPTION_SEARCH_FIELD_3 = "ddl_PD_Search_3";
        public const string PRODUCT_DESCRIPTION_SEARCH_FIELD_5 = "ddl_PD_Search_5";
        public const string PRODUCT_DESCRIPTION_SEARCH_FIELD_6 = "ddl_PD_Search_6";

        public const string PRODUCT_DESCRIPTION_TEXT_DDL_FIELD_07 = "ddl_PD_Text_DDL_07";
        public const string PRODUCT_DESCRIPTION_TEXT_DDL_FIELD_08 = "ddl_PD_Text_DDL_08";
        public const string PRODUCT_DESCRIPTION_TEXT_DDL_FIELD_09 = "ddl_PD_Text_DDL_09";
        public const string PRODUCT_DESCRIPTION_TEXT_DDL_FIELD_10 = "ddl_PD_Text_DDL_10";
        public const string PRODUCT_DESCRIPTION_TEXT_DDL_FIELD_11 = "ddl_PD_Text_DDL_11";
        public const string PRODUCT_DESCRIPTION_TEXT_DDL_FIELD_12 = "ddl_PD_Text_DDL_12";
        public const string PRODUCT_DESCRIPTION_STATUS_FIELD = "ddl_PD_Status";

        //Transaction
        public const string TRANSACTION_TYPE_Sale = "Sale";
        public const string TRANSACTION_TYPE_Return = "Return";

        public const string TRANSACTION_STATUS_InTransaction = "InTransaction";
        public const string TRANSACTION_STATUS_Hold = "HOLD";
        public const string TRANSACTION_STATUS_AllVoid = "ALLVOID";
        public const string TRANSACTION_STATUS_Confirmed = "Confirmed";

        public const string TRANSACTION_ITEM_STATUS_InTransaction = "InTransaction";
        public const string TRANSACTION_ITEM_STATUS_Hold = "HOLD";
        public const string TRANSACTION_ITEM_STATUS_Void = "VOID";
        public const string TRANSACTION_ITEM_STATUS_Confirmed = "Confirmed";
        public const string TRANSACTION_ITEM_Type_Deposit = "Deposit";  //Oct 31, 2013
        public const string TRANSACTION_ITEM_Type_EHF = "EHF";  //Oct 31, 2013
        public const string TRANSACTION_ITEM_Type_CRF = "CRF";  //Nov 4, 2013
        public const string TRANSACTION_ITEM_Type_TaxFree = "TaxFree";  //Nov 4, 2013

        //****Don't named the type as the same name*****
        public const string TRANSACTION_ITEM_Type_Item = "Item";
        public const string TRANSACTION_ITEM_Type_ItemDiscount = "ItemDiscount";
        public const string TRANSACTION_ITEM_Type_SubTotalDiscount = "SubtotalDiscount";
        public const string TRANSACTION_ITEM_Type_VIPDiscount = "VIPDiscount";// kinds of subtotal discount
        public const string TRANSACTION_ITEM_Type_TaxIncluded = "TaxIncluded";
        public const string TRANSACTION_ITEM_Type_DollarDiscount = "DollarDiscount";

        public const string TRANSACTION_ITEM_PriceOverrideFlag_True = "True";
        public const string TRANSACTION_ITEM_PriceOverrideFlag_False = "False";


        ////****Don't named the discountItemStatus same with normalItemStatus*****
        //public const string TRANSACTION_ITEM_STATUS_ItemDiscountInTransaction = "ItemDiscountInTransaction";
        //public const string TRANSACTION_ITEM_STATUS_ItemDiscountConfirmed = "ItemDiscountConfirmed";
        //public const string TRANSACTION_ITEM_STATUS_ItemDiscountHold = "ItemDiscountHOLD";
        
        //Tax
        public const string TAX_Available_FLAG_Yes = "Yes";
        public const string TAX_Available_FLAG_No = "No";

        //Payment
        public const string PAYMENT_TYPE_Payment = "Payment";
        public const string PAYMENT_TYPE_Refund = "Refund";

        public const string PAYMENT_METHOD_Cash = "Cash";
        public const string PAYMENT_METHOD_Debit = "Debit";
        public const string PAYMENT_METHOD_Visa = "Visa";
        public const string PAYMENT_METHOD_MasterCard = "MasterCard";
        public const string PAYMENT_METHOD_AE = "AmericanExpress";
        public const string PAYMENT_METHOD_JCB = "JCB";
        public const string PAYMENT_METHOD_StoreCredit = "StoreCredit";
        public const string PAYMENT_METHOD_USD = "USD";
        public const string PAYMENT_METHOD_AccountReceivable = "A/R";
        public const string PAYMENT_METHOD_Others = "Others";

        public const string PAYMENT_STATUS_Hold = "HOLD";
        public const string PAYMENT_STATUS_InTransaction = "InTransaction";
        public const string PAYMENT_STATUS_Confirmed = "Confirmed";
        public const string PAYMENT_STATUS_Void = "VOID";

        public const string btnHoldAndRecall_TEXT_Hold = "HOLD";
        public const string btnHoldAndRecall_TEXT_Recall = "RE CALL";

        //ReceiptPrint
        public const string RECEIPT_ID_CustomerCopy = "R1";
        public const string RECEIPT_TEST_VIEW_TransactionNo = "0123456789212345";

        public const string WARNING_TITLE_TEXT_Reprint = "Duplicate";
        public const string WARNING_TITLE_TEXT_DayEndReport = "Day End Report";
        public const string WARNING_TITLE_TEXT_SoldProductReport = "Item Sold Report";
        public const string WARNING_TITLE_TEXT_ReturnProductReport = "Item Return Report";
        public const string WARNING_TITLE_TEXT_Purchase = "Purchase";

        //Customer
        public const string CUSTOMER_STATUS_Available = "Available";
        public const string CUSTOMER_STATUS_Deleted = "Deleted";
        public const string CUSTOMER_NUMBER_Deleted = "Deleted";
        public const string CUSTOMER_Search_Criteria_Name = "Name";
        public const string CUSTOMER_Search_Criteria_IDNumber = "ID#";
        public const string CUSTOMER_Search_Criteria_Phone = "Phone";
        public const string CUSTOMER_Hello = "Hello";

        //Customer
        public const string USER_STATUS_Available = "Available";
        public const string USER_STATUS_Deleted = "Deleted";
        public const string USER_STATUS_Inactive = "Inactive";
        public const string USER_Search_Criteria_Name = "Name";
        public const string USER_Search_Criteria_SIN = "SIN";
        public const string USER_Search_Criteria_Phone = "Phone";
        public const string USER_Search_Criteria_LoginID = "Login ID";

        //Customer Discount Policy
        public const string DiscountPolicy_STATUS_Available = "Active";
        public const string DiscountPolicy_STATUS_Deleted = "Deleted";
        public const string DiscountPolicy_STATUS_Inactive = "Inactive";


        //ConsignorReceipt
        public const string CONSIGNOR_RECEIPT_STATUS_Confirmed = "Confirmed";

        //Scroll control
        public const int WM_SCROLL = 276; // Horizontal scroll
        public const int WM_VSCROLL = 277; // Vertical scroll
        public const int SB_LINEUP = 0; // Scrolls one line up
        public const int SB_LINELEFT = 0;// Scrolls one cell left
        public const int SB_LINEDOWN = 1; // Scrolls one line down
        public const int SB_LINERIGHT = 1;// Scrolls one cell right
        public const int SB_PAGEUP = 2; // Scrolls one page up
        public const int SB_PAGELEFT = 2;// Scrolls one page left
        public const int SB_PAGEDOWN = 3; // Scrolls one page down
        public const int SB_PAGERIGTH = 3; // Scrolls one page right
        public const int SB_PAGETOP = 6; // Scrolls to the upper left
        public const int SB_LEFT = 6; // Scrolls to the left
        public const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right
        public const int SB_RIGHT = 7; // Scrolls to the right
        public const int SB_ENDSCROLL = 8; // Ends scroll

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);



        //System Setting
        public const string SYSTEM_CONFIG_KEY_NAME_VersionType = "VersionType";
        public const string SYSTEM_CONFIG_KEY_NAME_DepartmentCategory = "DepartmentCategory";
        public const string SYSTEM_CONFIG_KEY_NAME_Weigh = "Weigh";

        public const string SYSTEM_CONFIG_VALUE_VERSION_TYPE_Consignor = "ConsignorVersion";
        public const string SYSTEM_CONFIG_VALUE_VERSION_TYPE_BookStore = "BookStoreVersion";
        public const string SYSTEM_CONFIG_VALUE_DepartmentCategory_Enable = "Enable";
        public const string SYSTEM_CONFIG_VALUE_DepartmentCategory_Disable = "Disable";
        public const string SYSTEM_CONFIG_VALUE_Weigh_Enable = "Enable";
        public const string SYSTEM_CONFIG_VALUE_Weigh_Disable = "Disable";


        //Department and Category //****Don't named the same name.*****
        public const string CREATE_NEW_Department = "NewDepartment";
        public const string CREATE_NEW_Category = "NewCategory";
        public const string UPDATE_Department = "UpdateDepartment";
        public const string UPDATE_Category = "UpdateCategory";

        //UI Label Display //****Don't named the same name.*****
        public const string UI_PRODUCT_DESCRIPTION_lbSearch1 = "PRODUCT_DESCRIPTION_lbSearch1";
        public const string UI_PRODUCT_DESCRIPTION_lbSearch2 = "PRODUCT_DESCRIPTION_lbSearch2";
        public const string UI_PRODUCT_DESCRIPTION_lbSearch3 = "PRODUCT_DESCRIPTION_lbSearch3";
        public const string UI_PRODUCT_DESCRIPTION_lbSearch4 = "PRODUCT_DESCRIPTION_lbSearch4";
        public const string UI_PRODUCT_DESCRIPTION_lbSearch5 = "PRODUCT_DESCRIPTION_lbSearch5";
        public const string UI_PRODUCT_DESCRIPTION_lbSearch6 = "PRODUCT_DESCRIPTION_lbSearch6";
        public const string UI_PRODUCT_DESCRIPTION_lbText01 = "PRODUCT_DESCRIPTION_lbText01";
        public const string UI_PRODUCT_DESCRIPTION_lbText02 = "PRODUCT_DESCRIPTION_lbText02";
        public const string UI_PRODUCT_DESCRIPTION_lbText03 = "PRODUCT_DESCRIPTION_lbText03";
        public const string UI_PRODUCT_DESCRIPTION_lbText04 = "PRODUCT_DESCRIPTION_lbText04";
        public const string UI_PRODUCT_DESCRIPTION_lbText05 = "PRODUCT_DESCRIPTION_lbText05";
        public const string UI_PRODUCT_DESCRIPTION_lbText06 = "PRODUCT_DESCRIPTION_lbText06";
        public const string UI_PRODUCT_DESCRIPTION_lbTextDDL07 = "PRODUCT_DESCRIPTION_lbTextDDL07";
        public const string UI_PRODUCT_DESCRIPTION_lbTextDDL08 = "PRODUCT_DESCRIPTION_lbTextDDL08";
        public const string UI_PRODUCT_DESCRIPTION_lbTextDDL09 = "PRODUCT_DESCRIPTION_lbTextDDL09";
        public const string UI_PRODUCT_DESCRIPTION_lbTextDDL10 = "PRODUCT_DESCRIPTION_lbTextDDL10";
        public const string UI_PRODUCT_DESCRIPTION_lbTextDDL11 = "PRODUCT_DESCRIPTION_lbTextDDL11";
        public const string UI_PRODUCT_DESCRIPTION_lbTextDDL12 = "PRODUCT_DESCRIPTION_lbTextDDL12";
        public const string UI_PRODUCT_DESCRIPTION_lbDecimal1 = "PRODUCT_DESCRIPTION_lbDecimal1";
        public const string UI_PRODUCT_DESCRIPTION_lbDecimal2 = "PRODUCT_DESCRIPTION_lbDecimal2";
        public const string UI_PRODUCT_DESCRIPTION_lbDecimal3 = "PRODUCT_DESCRIPTION_lbDecimal3";
        public const string UI_PRODUCT_DESCRIPTION_lbDecimal4 = "PRODUCT_DESCRIPTION_lbDecimal4";
        public const string UI_PRODUCT_DESCRIPTION_lbInt1 = "PRODUCT_DESCRIPTION_lbInt1";
        public const string UI_PRODUCT_DESCRIPTION_lbInt2 = "PRODUCT_DESCRIPTION_lbInt2";
        public const string UI_PRODUCT_DESCRIPTION_lbInt3 = "PRODUCT_DESCRIPTION_lbInt3";
        public const string UI_PRODUCT_DESCRIPTION_lbInt4 = "PRODUCT_DESCRIPTION_lbInt4";
        public const string UI_PRODUCT_DESCRIPTION_lbTF1 = "PRODUCT_DESCRIPTION_lbTF1";
        public const string UI_PRODUCT_DESCRIPTION_lbTF2 = "PRODUCT_DESCRIPTION_lbTF2";
        public const string UI_PRODUCT_DESCRIPTION_lbTF3 = "PRODUCT_DESCRIPTION_lbTF3";
        public const string UI_PRODUCT_DESCRIPTION_lbTF4 = "PRODUCT_DESCRIPTION_lbTF4";
        public const string UI_PRODUCT_DESCRIPTION_lbTF5 = "PRODUCT_DESCRIPTION_lbTF5";
        public const string UI_PRODUCT_DESCRIPTION_lbDateTime1 = "PRODUCT_DESCRIPTION_lbDateTime1";
        public const string UI_PRODUCT_DESCRIPTION_lbDateTime2 = "PRODUCT_DESCRIPTION_lbDateTime2";
        public const string UI_PRODUCT_DESCRIPTION_lbDateTime3 = "PRODUCT_DESCRIPTION_lbDateTime3";
        public const string UI_PRODUCT_DESCRIPTION_lbDateTime4 = "PRODUCT_DESCRIPTION_lbDateTime4";
        public const string UI_PRODUCT_DESCRIPTION_lbStatus = "PRODUCT_DESCRIPTION_lbStatus";
        public const string UI_PRODUCT_DESCRIPTION_lbFilePath = "PRODUCT_DESCRIPTION_lbFilePath";


        public enum Promotion
        {
            MixMatch = 0,
            Combo = 1,
            QuantityDiscount=2,
            BOM = 3,
            WeeklySpecial = 4,
            DailySpecial = 5,
            TaxFree = 6
        }
    }


}
