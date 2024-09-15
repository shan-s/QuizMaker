Quiz Maker is a GenAI tool that uses RAG to generate quiz on the fly based on content uploaded. It is an ASP.NET web application that utilises Semantic Kernel and Kernel Memory to invoke GPT-4o to produce structured JSON output based on unstructured course/book content that is uploaded by the user. 

The JSON format quiz is then rendered in the form of an multiple-choice question in a web page which be then submitted by the student. Evaluation is also inbuilt into the application.

Uses the below Platforms & Technologies:
1) LLM Orchestration - Microsoft Semantic Kernel
2) Vector Database - Azure AI Search
3) Connector for Vector Database - Microsoft Kernel Memory
4) LLM Deployment & Content Filtering -  Azure AI Studio
5) Database - CosmosDB MongoDB
