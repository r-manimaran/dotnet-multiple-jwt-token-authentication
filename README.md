# Multi JWT Auth to a single .net Web API
This .net project uses multiple jwt token for authentication in the same web api poject. Uses from any of the below two providers can login to the app and access the endpoints.

  1. Keycloak Authentication
  2. Supabase Authetication  
 
**Install the packages**:

- Microsoft.AspNetCore.Authentication.JwtBearer
  

**Get Keycloak Jwt using Postman**

![alt text](image.png) 

**KeyCloak authentication in .Net app with Access Token**

![alt text](image-1.png)

**Get Supabase Token using Postman**

![alt text](image-2.png)

**Supbase authentication in .Net app with Token**

![alt text](image-3.png)

**Adding Custom Claim to have the Source provider information**

![alt text](image-4.png)

![alt text](image-5.png)