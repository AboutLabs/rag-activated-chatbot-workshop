# Document Processing Exercise

This exercise demonstrates document processing using Azure Form Recognizer.

## Features
- Text extraction from PDF, Word, and image documents
- Document normalization and cleaning
- Metadata handling
- Azure Form Recognizer integration

## Prerequisites
- Azure Form Recognizer resource
- .NET 6.0 SDK
- Azure subscription

## Setup
1. Update appsettings.json with your Azure credentials
2. Build and run the project
3. Test with sample documents

## Usage
```csharp
var analyzer = new AzureDocumentAnalyzer(endpoint, key);
var extractor = new TextExtractor(analyzer);
var processedDoc = await extractor.ProcessDocumentAsync(filePath, metadata);

This implementation provides:
- Document text extraction
- Text normalization
- Metadata handling
- Error handling
- Azure Form Recognizer integration
- Clean architecture with separation of concerns

The code is structured for easy understanding and modification during the workshop.