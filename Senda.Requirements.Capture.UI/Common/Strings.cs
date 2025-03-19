using System;
using System.Collections.Generic;
using System.Text;

namespace Senda.Requirements.Capture.UI.Common
{

    /// <summary>
    /// These values must be kept in sync with the "reference_key" values in the Requirements database
    /// in the corresponding tables.
    /// </summary>
    public struct Strings    
    {
        public struct SubjectTypes
        {
            public struct ReferenceKeys
            {
                public const string AwardLine = "AwardLine";
                public const string Customer = "Customer";
                public const string OpportunityLine = "OpportunityLine";
                public const string VendorPart = "VendorPart";
                public const string Award = "Award";
                public const string Vendor = "Vendor";
                public const string Opportunity = "Opportunity";
                public const string NSN = "NSN";
            }
        }
        public struct Context
        {
            public struct ReferenceKeys
            {
                public const string PreAwardRfqCreation = "PreAward.Rfq.Creation";
                public const string PreAwardAwardReview = "PreAward.Award.Review";
                public const string PostAwardPurchaseOrderCreation = "PostAward.PurchaseOrder.Creation";
                public const string PostAwardWorkOrderCreation = "PostAward.WorkOrder.Creation";
                public const string WarehouseReceivingAlert = "Warehouse.Receiving.Alert";
                public const string WarehousePackagingAlert = "Warehouse.Packaging.Alert";
                public const string WarehouseShippingAlert = "Warehouse.Shipping.Alert";
                public const string WarehouseInspectionAlert = "Warehouse.Inspection.Alert";
            }
        }
        public struct SourceTypes
        {
            public struct ReferenceKeys
            {
                public const string WebUrl = "WebUrl";
                public const string Person = "Person";
                public const string PdfFile = "PdfFile";
                public const string ExcelFile = "ExcelFile";
                public const string WordFile = "WordFile";
                public const string Email = "Email";                
            }
        }
        public struct Topics
        {
            public struct ReferenceKeys
            {
                public const string HazMat = "HazMat";
                public const string CertOfQualityCompliance = "CertOfQualityCompliance";
                public const string FirstArtInspection = "FirstArtInspection";
                public const string PurchaseRequestNumber = "PurchaseRequestNumber";
                public const string ShelfLife = "ShelfLife";
                public const string SpecialPackagingInstruction = "SpecialPackagingInstruction";
                public const string CertOfCompliance = "CertOfCompliance";
                public const string FirstArtTest = "FirstArtTest";
                public const string DeliveryDate = "DeliveryDate";
                public const string FinalInspectionAtManufacturer = "FinalInspectionAtManufacturer";
                public const string ProductLotTest = "ProductLotTest";
            }
        }

    }
}
