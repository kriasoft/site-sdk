CREATE TABLE [Membership].[Credential] (
    [UserID] INT           NOT NULL, -- 'User identification number. Foreign key to User.UserID.
    [TypeID] TINYINT       NOT NULL, -- Credential type identification number. Foreign key to CredentialType.TypeID.
    [Hash]   NVARCHAR (50) NOT NULL,
    [Salt]   NVARCHAR (50) NULL,
    CONSTRAINT [PK_Credential_UserID_TypeID] PRIMARY KEY CLUSTERED ([UserID] ASC, [TypeID] ASC),
    CONSTRAINT [FK_Credential_CredentialType] FOREIGN KEY ([TypeID]) REFERENCES [Membership].[CredentialType] ([TypeID]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_Credential_User] FOREIGN KEY ([UserID]) REFERENCES [Membership].[User] ([UserID]) ON DELETE CASCADE ON UPDATE CASCADE
);

