# AuthMicroservice API

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or Docker)
- SendGrid API Key (for emails)

### Running Locally

1. **Update Configuration**

   ```bash
   # Update appsettings.Development.json with your settings
   cp appsettings.json appsettings.Development.json
   ```

2. **Run the API**

   ```bash
   dotnet run
   ```

3. **Access Swagger UI**
   - Navigate to: `https://localhost:7000`

### Using Docker

1. **Build and Run**

   ```bash
   docker-compose up --build
   ```

2. **Access API**
   - API: `https://localhost:5001`
   - HTTP: `http://localhost:5000`

## 📡 API Endpoints

### Authentication

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - User logout

### Email Verification

- `POST /api/emailverification/send` - Send verification email
- `POST /api/emailverification/verify` - Verify email

### Password Reset

- `POST /api/passwordreset/forgot-password` - Request password reset
- `POST /api/passwordreset/reset-password` - Reset password

### Social Authentication

- `POST /api/socialauth/google` - Google OAuth login
- `POST /api/socialauth/github` - GitHub OAuth login
- `GET /api/socialauth/google/url` - Get Google auth URL
- `GET /api/socialauth/github/url` - Get GitHub auth URL

### User Profile

- `GET /api/userprofile` - Get user profile
- `PUT /api/userprofile` - Update user profile
- `POST /api/userprofile/change-password` - Change password

### Health Check

- `GET /health` - API health status

## 🔧 Configuration

### Required Environment Variables

```bash
# Database
ConnectionStrings__DefaultConnection="Server=localhost;Database=AuthMicroserviceDB;Trusted_Connection=true;"

# JWT
JwtSettings__SecretKey="YourSecretKeyHere"
JwtSettings__Issuer="AuthMicroservice"
JwtSettings__Audience="AuthMicroservice-Users"

# SendGrid
EmailSettings__ApiKey="YOUR_SENDGRID_API_KEY"
EmailSettings__FromEmail="noreply@yourapp.com"

# Social Auth
SocialAuth__GoogleClientId="YOUR_GOOGLE_CLIENT_ID"
SocialAuth__GoogleClientSecret="YOUR_GOOGLE_CLIENT_SECRET"
SocialAuth__GitHubClientId="YOUR_GITHUB_CLIENT_ID"
SocialAuth__GitHubClientSecret="YOUR_GITHUB_CLIENT_SECRET"
```

## 🏗️ Architecture

- **Clean Architecture** with Domain, Application, Infrastructure, and API layers
- **CQRS** pattern with MediatR
- **JWT Authentication** with refresh tokens
- **Global Exception Handling**
- **Comprehensive Logging** with Serilog
- **Health Checks** for monitoring

## 📚 Features

✅ User Registration & Authentication  
✅ Email Verification  
✅ Password Reset  
✅ JWT with Refresh Tokens  
✅ Social Login (Google, GitHub)  
✅ User Profile Management  
✅ Global Exception Handling  
✅ Comprehensive Logging  
✅ Health Checks  
✅ Docker Support  
✅ Swagger Documentation

## 🔒 Security

- Passwords hashed with BCrypt
- JWT tokens with configurable expiration
- Refresh token rotation
- Rate limiting ready
- CORS configured
- HTTPS enforced in production

## 📊 Monitoring

- Health checks at `/health`
- Structured logging with Serilog
- Request/Response logging
- Exception tracking
