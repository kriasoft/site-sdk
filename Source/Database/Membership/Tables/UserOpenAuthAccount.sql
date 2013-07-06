CREATE TABLE [Membership].[UserLogin] (
    [UserID]           INT            NOT NULL,
    [ProviderName]     NVARCHAR (24)  NOT NULL,
    [ProviderUserID]   NVARCHAR (128) NOT NULL,
    [ProviderUserName] NVARCHAR (128) NOT NULL,
    [LastUsedDate]     DATETIME       NULL,
    CONSTRAINT [PK_UserLogin_ProviderName_ProviderUserID] PRIMARY KEY CLUSTERED ([ProviderName] ASC, [ProviderUserID] ASC),
    CONSTRAINT [FK_UserLogin_User] FOREIGN KEY ([UserID]) REFERENCES [Membership].[User] ([UserID])
);

GO
CREATE NONCLUSTERED INDEX [IX_UserLogin_UserID_ProviderName_ProviderID]
    ON [Membership].[UserLogin]([UserID] ASC, [ProviderName] ASC, [ProviderUserID] ASC);
