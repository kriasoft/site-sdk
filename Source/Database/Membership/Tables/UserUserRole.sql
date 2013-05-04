CREATE TABLE [Membership].[UserUserRole] (
    [UserID] INT     NOT NULL,
    [RoleID] TINYINT NOT NULL,
    CONSTRAINT [PK_UserUserRole_UserID_RoleID] PRIMARY KEY CLUSTERED ([UserID] ASC, [RoleID] ASC),
    CONSTRAINT [FK_UserUserRole_User] FOREIGN KEY ([UserID]) REFERENCES [Membership].[User] ([UserID]),
    CONSTRAINT [FK_UserUserRole_UserRole] FOREIGN KEY ([RoleID]) REFERENCES [Membership].[UserRole] ([RoleID])
);

