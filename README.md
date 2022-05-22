Sample files, scripts, templates and projects for presentation "Breaking a Monolith Data Import into Azure Logic Apps Quick".

[Watch the full livestream!](https://www.youtube.com/watch?v=QRb0aP9qVXg)

# Introduction
Everyone probably has made them: Scheduled tasks to download files and import the data into a different database after some transformations and business logic. But those can get vulnerable quickly by piling up its purposes and external dependencies, creating a small monolith. Azure has different resource types to help you create these kind of tasks.

But if you start from an existing one, you can use Azure Logic Apps to offload your external connections without any code. Combine it with Azure Storage Accounts and Service Bus to create a whole extendable integration solution. We’ll start with an Azure Function that was responsible for all steps of an import, and break it down with Logic Apps to the point where it can ingest files from different sources at the same time.

# General
Basic ARM template with parts of the Azure resources that will be used throughout the presentation.

# Phase 0
The initial starting Azure Function application.

# Phase 1
The first rework of the Azure Function application and the first Azure Logic App to abstract away the download process.

# Phase 2
The next Azure Logic App to add the alternative download source, without touching the Azure Function.

# Phase 3
The last change to the Azure Function and last Azure Logic App to add the new requirement.