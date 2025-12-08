-- Insert Admin Role
INSERT INTO Roles (Name, CreatedAt) VALUES ('Admin', GETDATE());

-- Insert Admin User
INSERT INTO Users (Username, Email, Password, FullName, IsActive, CreatedAt)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin User', 1, GETDATE());

-- Link User to Admin Role
DECLARE @AdminRoleId INT = (SELECT Id FROM Roles WHERE Name = 'Admin');
DECLARE @AdminUserId INT = (SELECT Id FROM Users WHERE Username = 'admin');

INSERT INTO UserRoles (UserId, RoleId, Id, CreatedAt)
VALUES (@AdminUserId, @AdminRoleId, 1, GETDATE());

-- Insert some sample categories
INSERT INTO Categories (Name, Description, IsActive, CreatedAt)
VALUES
('Genel', 'Genel sorular', 1, GETDATE()),
('Teknoloji', 'Teknoloji ile ilgili sorular', 1, GETDATE()),
('Yazılım', 'Yazılım geliştirme soruları', 1, GETDATE());
