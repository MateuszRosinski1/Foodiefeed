# Foodiefeed

**Foodiefeed** is a project developed as my engineering thesis. It is an application for sharing culinary recipes. This work consists of two projects:  
- **Foodiefeed**: the client application, implemented in **.NET MAUI**.  
- **Foodiefeed_api**: the application API responsible for the backend in **ASP.NET**.

## Architecture

### Foodiefeed  
The client application is built using **.NET MAUI**, targeting both **Windows** and **Android** platforms. It incorporates the **Community Toolkit for MAUI**, which provides MVVM helpers, animations, converters, and custom controls to enhance development efficiency and user experience. The application follows the **MVVM pattern** for a clean and scalable architecture.

### Foodiefeed_api  
The backend API, **Foodiefeed_api**, is developed in **ASP.NET**. It handles all server-side logic and data management. Key technologies and packages used include:  
- **Entity Framework Core** for database management and querying.  
- **FuzzySharp** for advanced string matching and search functionality.  
- **Bogus** for generating mock data during development.  

The API is deployed to **Azure Cloud** and is accessible as a public endpoint, ensuring reliability and scalability.

## Future Plans

What could I implement in the future? Planned features include:  
- **JWT Token Authentication** for secure user authentication.  
- **Real-time chat** for communication using the **SignalR framework**.  
