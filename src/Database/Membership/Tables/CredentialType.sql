CREATE TABLE [Membership].[CredentialType] (
    [TypeID] TINYINT      NOT NULL,
    [Name]   [dbo].[Name] NOT NULL,
    CONSTRAINT [PK_CredentialType_TypeID] PRIMARY KEY CLUSTERED ([TypeID] ASC)
);

