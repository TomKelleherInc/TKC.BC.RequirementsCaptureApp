# Requirements Capture App

This is an application I created for a former employer that allowed their internal staff to more rapidly 
look over huge RFP PDF files (100-200 pages) from the Department of Defense, and quickly search for key data points,
select and capture them to a shared management database, annotate the PDF files, and store all of them back
in a central storage for future use.  

The PDFs contained huge amounts of irrelevant boilerplate text.  Users would have to find a handful of needles in a huge government-issued haystack.  Often they would find 40 pages of boilerplate between the sections they needed.  So a key success factor was to give them the power to search for certain specific data points across these huge documents, capturing that data, and moving at speed through (sometimes) *hundreds* of RFPs per day.  

The design elements for this tool included:

- In-house use only
- Needed to display the PDF file within the UI, and allow selecting/copying from it.
- Allowed users to search for key data types using common phrases. For example, "Delivery Date" might have the title of "del date" or "del. date" or "deliv. dt" etc., and this system would allow the team to collect as many such idiosyncratic abbreviations as they needed, and capture them all under the concept of "delivery-date."  This allowed them to quickly sweep through the entire PDF, searching for all those variations, and then jumping to the search-hits.  As new "concepts" were created and multiple search-strings defined for it, these were all shared across all users, so that the improvements made by one user were enjoyed by all.
- When the user found a key piece of information, they could copy/paste it into the master record for that RFP, storing it in a shared microservice database, and making sure anyone else working this RFP in the future had those details readily at hand.
- The system needed to allow users to search for RFPs based on RFP numbers, and to quickly see if someone else had already processed it.
- When a piece of data was found and captured, the system would annotate the underlying PDF and save it back to the central repository.  The annotation would include the date, the person's name, the data collected and what it was for (e.g., "delivery-date").
- The system needed to work with existing microservices and RFP database tools.

# Technologies

## Frontend

- WPF "smart client"
- DevExpress component library (especially for the PDF viewer functionality)
- ClickOnce technology for easy deployment and updates

## Backend

- Postgres Database & Web API technology
- Entity Framework for database access
- Data Transport Objects (DTOs) for sharing data between front/back ends.


# Projects

## Sedna.Service.Requirements.API.Client.Test

The initial framework for creating Unit Tests.  Work interrupted by Covid and other matters.

## Sedna.Service.Requirements.API.Client


## Sedna.Service.Requirements.API

Backend API for interacting with other company microservices, and the frontend

## Sedna.Service.Requirements.Core

(Placeholder project that was never populated with code.)

## Sedna.Service.Requirements.DTO

Data Transport Objects, used as a reference by both the front/back ends.   Provided common language for communicating between APIs and front end.


## Sedna.Web.Requirements.UI

Abandoned attempt to code all this as a web interface.  Experimented for a few days with a library called "PDF.js" to read and annotate PDFs, but the display was inconsistent and any customization would have been time prohibitive.  

Although we had a principle on the team to build all tools as web applications, we allowed this exception given the narrow and well-understood user population, the superiority of DevExpress's PDF viewer, and the easy updates 
possible using ClickOnce deployment.

## Senda.Requirements.Capture.UI

The final UI layer, using DevExpress.
