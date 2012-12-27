CREATE TABLE [Membership].[UserRole] (
    [RoleID] TINYINT      NOT NULL,
    [Name]   [dbo].[Name] NOT NULL,
    CONSTRAINT [PK_UserRole_RoleID] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);

