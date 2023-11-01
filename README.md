# Image Uploader

Image Uploader is a web application developed using ASP.NET Web API 8 Preview. It serves as a powerful image uploading and storage solution. The application uses RabbitMQ for efficient image queueing and leverages Azure Blob Storage for reliable image storage. Additionally, it saves the Azure Blob Storage URLs in a MongoDB database for easy retrieval and management.

# Features
## Image Queueing: 
Image Uploader employs RabbitMQ to efficiently queue incoming images, ensuring that image uploads are processed in a controlled and orderly manner.

## Azure Blob Storage: 
Images are stored in Azure Blob Storage, providing a secure and scalable solution for image storage. This ensures the reliability and availability of your uploaded images.

## MongoDB Integration: 
The application integrates with a MongoDB database to store the URLs of the images stored in Azure Blob Storage. This allows for easy retrieval and management of the uploaded images.

## ASP.NET Web API 8 Preview: 
The use of the latest ASP.NET Web API 8 Preview ensures a modern and efficient web API for image uploading.

# Tech Stacks:
- ASP.NET Core WEB API 8 Preview
- RabbitMQ
- MongoDB
- Docker
- Azure Blob

# Architechture
![Example Image](C:\Users\rajen\Downloads\Picture1.png)


