CREATE TABLE Users (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(MAX) NOT NULL ,
    Email NVARCHAR(MAX),
    PasswordHash NVARCHAR(MAX) NOT NULL,
    DeviceId NVARCHAR(MAX),                -- Stores current device ID
    JwtSessionId UNIQUEIDENTIFIER,         -- Tracks the active session
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME DEFAULT GETDATE(),
	CreatedBy NVARCHAR(MAX),
	UpdatedOn DATETIME DEFAULT GETDATE(),
	UpdatedBy NVARCHAR(MAX),
    LastLogin DATETIME
);

INSERT INTO Users (Username, Email, PasswordHash, DeviceId, JwtSessionId, CreatedBy, UpdatedBy, LastLogin)
VALUES 
('user1', 'user1@example.com', 'hash_password1', 'device1', NEWID(), 'System', 'System', GETDATE()),
('user2', 'user2@example.com', 'hash_password2', 'device2', NEWID(), 'System', 'System', GETDATE()),
('user3', 'user3@example.com', 'hash_password3', 'device3', NEWID(), 'System', 'System', GETDATE()),
('user4', 'user4@example.com', 'hash_password4', 'device4', NEWID(), 'System', 'System', GETDATE()),
('user5', 'user5@example.com', 'hash_password5', 'device5', NEWID(), 'System', 'System', GETDATE()),
('user6', 'user6@example.com', 'hash_password6', 'device6', NEWID(), 'System', 'System', GETDATE()),
('user7', 'user7@example.com', 'hash_password7', 'device7', NEWID(), 'System', 'System', GETDATE()),
('user8', 'user8@example.com', 'hash_password8', 'device8', NEWID(), 'System', 'System', GETDATE()),
('user9', 'user9@example.com', 'hash_password9', 'device9', NEWID(), 'System', 'System', GETDATE()),
('user10', 'user10@example.com', 'hash_password10', 'device10', NEWID(), 'System', 'System', GETDATE());


CREATE TABLE [dbo].[UserSessions] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [UserId] BIGINT NOT NULL,
	[Username] NVARCHAR(MAX) NOT NULL,
    [JwtToken] NVARCHAR(MAX) NOT NULL,
    [IssuedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
