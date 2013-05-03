CREATE TABLE [Membership].[UserOpenAuthAccount] (
    [UserID]           INT            NOT NULL,
    [ProviderName]     NVARCHAR (24)  NOT NULL,
    [ProviderUserID]   NVARCHAR (64)  NOT NULL,
    [ProviderUserName] NVARCHAR (128) NOT NULL,
    [LastUsedDate]     DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ProviderName] ASC, [ProviderUserID] ASC), 
    CONSTRAINT [FK_UserOpenAuthAccount_User] FOREIGN KEY ([UserID]) REFERENCES [Membership].[User]([UserID])
);
GO

CREATE INDEX [IX_UserOpenAuthAccount_UserID_ProviderName_ProviderID] ON [Membership].[UserOpenAuthAccount] ([UserID] ASC, [ProviderName] ASC, [ProviderUserID] ASC);

