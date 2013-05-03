CREATE TABLE [Membership].[UserRole] (
    [RoleID]   TINYINT      NOT NULL,
    [RoleName] [dbo].[Name] NOT NULL,
    CONSTRAINT [PK_UserRole_RoleID] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);

