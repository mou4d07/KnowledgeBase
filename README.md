Knowledge Base - Incidents & Guides
A comprehensive knowledge management system for tracking IT incidents and storing technical guides, built with Next.js and ASP.NET Core.

Overview
This application helps organizations document, track, and resolve incidents while maintaining a searchable library of technical guides and procedures.

Tech Stack
Component	Technology
Frontend	Next.js (React)
Backend API	ASP.NET Core
Database	SQL Server
Authentication	JWT
Styling	Tailwind CSS / Bootstrap
Features
Incidents Module
Create, view, update, and delete incidents

Assign priority levels (Low, Medium, High, Critical)

Track incident status (Open, In Progress, Resolved, Closed)

Assign to technicians

Add comments and solutions

Search and filter incidents

Guides Module
Create technical guides with rich text editor

Categorize guides by technology, department, or topic

Version tracking

Attach files and images

Rate and comment on guides

Search functionality

Additional Features
User authentication and roles (Admin, Technician, Viewer)

Dashboard with statistics

Export reports (PDF/Excel)

Activity logging

Email notifications

Prerequisites
Node.js (v18 or later)

.NET SDK (v6, v7, or v8)

SQL Server (2019 or later) or SQL Server Express

Git

Visual Studio 2022 / VS Code

Getting Started
1. Clone the repository
bash
git clone https://github.com/your-username/knowledge-base.git
cd knowledge-base
2. Backend Setup (ASP.NET Core)
bash
cd backend
dotnet restore
Update the connection string in appsettings.json:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=KnowledgeBaseDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "your-secret-key-here-minimum-32-characters",
    "Issuer": "knowledgebase-api",
    "Audience": "knowledgebase-clients"
  }
}
Run migrations and start the API:

bash
dotnet ef database update
dotnet run
The API runs at https://localhost:5001 | http://localhost:5000

3. Frontend Setup (Next.js)
Open a new terminal:

bash
cd frontend
npm install
Create a .env.local file:

text
NEXT_PUBLIC_API_URL=https://localhost:5001/api
Start the development server:

bash
npm run dev
The application runs at http://localhost:3000

Project Structure
text
knowledge-base/
├── backend/
│   ├── Controllers/
│   │   ├── IncidentsController.cs
│   │   ├── GuidesController.cs
│   │   └── AuthController.cs
│   ├── Models/
│   │   ├── Incident.cs
│   │   ├── Guide.cs
│   │   └── User.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Services/
│   └── Program.cs
│
├── frontend/
│   ├── app/
│   │   ├── incidents/
│   │   ├── guides/
│   │   ├── dashboard/
│   │   └── login/
│   ├── components/
│   ├── lib/
│   └── public/
│
└── database/
    └── migrations/
API Endpoints
Authentication
Method	Endpoint	Description
POST	/api/auth/login	User login
POST	/api/auth/register	User registration
Incidents
Method	Endpoint	Description
GET	/api/incidents	Get all incidents
GET	/api/incidents/{id}	Get incident by ID
POST	/api/incidents	Create new incident
PUT	/api/incidents/{id}	Update incident
DELETE	/api/incidents/{id}	Delete incident
GET	/api/incidents/status/{status}	Filter by status
Guides
Method	Endpoint	Description
GET	/api/guides	Get all guides
GET	/api/guides/{id}	Get guide by ID
POST	/api/guides	Create new guide
PUT	/api/guides/{id}	Update guide
DELETE	/api/guides/{id}	Delete guide
GET	/api/guides/search?q={query}	Search guides
Default Roles
Role	Permissions
Admin	Full access (CRUD all, user management)
Technician	Create, update incidents; view guides
Viewer	View incidents and guides only
Deployment
Backend Deployment
bash
cd backend
dotnet publish -c Release -o ./publish
Deploy the ./publish folder to your hosting server (IIS, Azure, AWS, etc.)

Frontend Deployment
bash
cd frontend
npm run build
npm run start
Or deploy to Vercel:

bash
vercel --prod
Database Deployment
Update connection string for production

Run migrations on production database:

bash
dotnet ef database update --connection "your_production_connection_string"
Environment Variables
Backend (appsettings.Production.json)
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=production-server;Database=KnowledgeBaseDB;User Id=sa;Password=your_password;"
  },
  "Jwt": {
    "Key": "production-secret-key",
    "Issuer": "knowledgebase-api",
    "Audience": "knowledgebase-clients"
  }
}
Frontend (.env.production)
text
NEXT_PUBLIC_API_URL=https://your-api-domain.com/api
Common Commands
Backend
bash
dotnet build              # Build the project
dotnet run               # Run the API
dotnet ef migrations add MigrationName   # Create migration
dotnet ef database update                # Apply migrations
dotnet test              # Run tests
Frontend
bash
npm run dev              # Development mode
npm run build            # Production build
npm run start            # Run production build
npm run lint             # Run linting
Troubleshooting
Database connection issues
Verify SQL Server is running

Check connection string in appsettings.json

Ensure Windows Authentication or SQL Auth is configured

JWT authentication errors
Verify JWT key is at least 32 characters

Check Issuer and Audience match in both backend and frontend

CORS issues
Ensure CORS is configured in Program.cs:

csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
Contributing
Fork the repository

Create a feature branch (git checkout -b feature/amazing-feature)

Commit changes (git commit -m 'Add amazing feature')

Push to branch (git push origin feature/amazing-feature)

Open a Pull Request

Contact
Mounir Boudmagh - boudmaghmounir@gmail.com

GitHub: your-username

Project Link: https://github.com/your-username/knowledge-base
