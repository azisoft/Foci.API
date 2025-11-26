CREATE DATABASE Foci
GO
USE Foci
GO

CREATE TABLE dbo.Todos (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Title           NVARCHAR(100) NOT NULL,
    Description     NVARCHAR(255) NULL,
    DueDate         DATETIME NULL,
    CreatedAt       DATETIME DEFAULT SYSDATETIME(),
    IsCompleted     BIT DEFAULT 0,
);
GO

INSERT INTO dbo.Todos (Title, Description, DueDate)
VALUES
('Create DB', 'Create a database that holds todos', '2025-02-05 17:00:00'),
('Create API', 'Create a .NET REST API to manage todos', '2025-02-10 23:59:00'),
('Add Swagger', 'Add OpenAPI Swagger to the API', '2025-03-01 10:30:00'),
('Create App', 'Create and Angular SPA App to display todos', '2025-02-07 14:00:00');
GO
