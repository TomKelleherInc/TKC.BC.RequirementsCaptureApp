export const requirements = 
[
    {
        "requirementId": 1,
        "sourceId": 1,
        "sourceText": "This is hazmat",
        "sourceTextLocation": "page:31",
        "preferredPhrasing": "This part is considered hazardous material",
        "topicId": 2,
        "subjectId": 2,
        "isActive": true,
        "reviewDt": "2020-07-31T00:00:00",
        "createdTs": "2020-02-11T18:01:44.107",
        "updatedTs": "2020-02-11T18:01:44.107",
        "createdUserId": 529,
        "updatedUserId": 529,
        "source": null,
        "subject": {
            "subjectId": 2,
            "externalIdentifier": "OPL000555",
            "subjectTypeId": 1,
            "tag": "{}",
            "description": " Solicitation Line id-555",
            "subjectType": null,
            "requirement": []
        },
        "topic": {
            "topicId": 2,
            "name": "Hazmat",
            "description": "This part is considered a hazardous material",
            "preferredPhrasing": "This part is considered hazardous material",
            "requirement": [],
            "subjectTypeTopic": [],
            "topicSearch": [
                {
                    "topicSearchId": 4,
                    "topicId": 2,
                    "searchString": "hazmat",
                    "description": null,
                    "isWholeWord": false
                },
                {
                    "topicSearchId": 5,
                    "topicId": 2,
                    "searchString": "haz-mat",
                    "description": null,
                    "isWholeWord": false
                },
                {
                    "topicSearchId": 6,
                    "topicId": 2,
                    "searchString": "hazardous material",
                    "description": null,
                    "isWholeWord": false
                }
            ]
        },
        "requirementContext": [
            {
                "requirementContextId": 8,
                "requirementId": 1,
                "key": "PreAward.Rfq.IncludeInRfq",
                "value": "true"
            },
            {
                "requirementContextId": 9,
                "requirementId": 1,
                "key": "PreAward.Bid.IncludeInBid",
                "value": "true"
            },
            {
                "requirementContextId": 10,
                "requirementId": 1,
                "key": "PostAward.PurchaseOrder.IncludeInPO",
                "value": "true"
            },
            {
                "requirementContextId": 11,
                "requirementId": 1,
                "key": "Warehouse.Receiving.Alert",
                "value": "true"
            },
            {
                "requirementContextId": 12,
                "requirementId": 1,
                "key": "Warehouse.Packaging.Alert",
                "value": "true"
            }
        ]
    },
    {
        "requirementId": 3,
        "sourceId": 1,
        "sourceText": "Delivery Dec 13 2020",
        "sourceTextLocation": "page:3",
        "preferredPhrasing": "Delivery must be on or before December 13, 2020",
        "topicId": 13,
        "subjectId": 1,
        "isActive": true,
        "reviewDt": "2020-07-31T00:00:00",
        "createdTs": "2020-02-11T18:01:44.107",
        "updatedTs": "2020-02-11T18:01:44.107",
        "createdUserId": 529,
        "updatedUserId": 529,
        "source": null,
        "subject": {
            "subjectId": 1,
            "externalIdentifier": "OPL0000400",
            "subjectTypeId": 1,
            "tag": "{}",
            "description": " Solicitation Line id-400",
            "subjectType": null,
            "requirement": []
        },
        "topic": {
            "topicId": 13,
            "name": "DeliveryDate",
            "description": "Customer's required delivery date",
            "preferredPhrasing": "Vendor must confirm that customer's delivery date (___) can be met.",
            "requirement": [],
            "subjectTypeTopic": [],
            "topicSearch": [
                {
                    "topicSearchId": 21,
                    "topicId": 13,
                    "searchString": "Delivery Date",
                    "description": null,
                    "isWholeWord": false
                },
                {
                    "topicSearchId": 22,
                    "topicId": 13,
                    "searchString": "Del. Date",
                    "description": null,
                    "isWholeWord": false
                }
            ]
        },
        "requirementContext": [
            {
                "requirementContextId": 13,
                "requirementId": 3,
                "key": "PreAward.Rfq.IncludeInRfq",
                "value": "true"
            },
            {
                "requirementContextId": 14,
                "requirementId": 3,
                "key": "PreAward.Bid.IncludeInBid",
                "value": "true"
            },
            {
                "requirementContextId": 15,
                "requirementId": 3,
                "key": "PostAward.PurchaseOrder.IncludeInPO",
                "value": "true"
            },
            {
                "requirementContextId": 16,
                "requirementId": 3,
                "key": "Warehouse.Receiving.Alert",
                "value": "true"
            }
        ]
    },
    {
        "requirementId": 5,
        "sourceId": 1,
        "sourceText": "Shelf-life req: 90%",
        "sourceTextLocation": "page:29",
        "preferredPhrasing": "The remaining shelf-life for this part will not be less than 90%",
        "topicId": 1,
        "subjectId": 2,
        "isActive": true,
        "reviewDt": "2020-07-31T00:00:00",
        "createdTs": "2020-02-11T18:01:44.107",
        "updatedTs": "2020-02-11T18:01:44.107",
        "createdUserId": 529,
        "updatedUserId": 529,
        "source": null,
        "subject": {
            "subjectId": 2,
            "externalIdentifier": "OPL000555",
            "subjectTypeId": 1,
            "tag": "{}",
            "description": " Solicitation Line id-555",
            "subjectType": null,
            "requirement": []
        },
        "topic": {
            "topicId": 1,
            "name": "ShelfLife",
            "description": "The specific shelf-life requirements for a line in a solicitation",
            "preferredPhrasing": "The remaining shelf-life for this part will not be less than __%",
            "requirement": [],
            "subjectTypeTopic": [],
            "topicSearch": [
                {
                    "topicSearchId": 1,
                    "topicId": 1,
                    "searchString": "shelf life",
                    "description": "",
                    "isWholeWord": false
                },
                {
                    "topicSearchId": 2,
                    "topicId": 1,
                    "searchString": "shelflife",
                    "description": null,
                    "isWholeWord": false
                },
                {
                    "topicSearchId": 3,
                    "topicId": 1,
                    "searchString": "shelf-life",
                    "description": null,
                    "isWholeWord": false
                }
            ]
        },
        "requirementContext": [
            {
                "requirementContextId": 17,
                "requirementId": 5,
                "key": "PreAward.Rfq.IncludeInRfq",
                "value": "true"
            },
            {
                "requirementContextId": 18,
                "requirementId": 5,
                "key": "PreAward.Bid.IncludeInBid",
                "value": "true"
            },
            {
                "requirementContextId": 19,
                "requirementId": 5,
                "key": "PostAward.PurchaseOrder.IncludeInPO",
                "value": "true"
            }
        ]
    }
]