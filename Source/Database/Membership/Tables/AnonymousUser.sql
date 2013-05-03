CREATE TABLE [Membership].[AnonymousUser] (
    [AnonymousUserID]  UNIQUEIDENTIFIER NOT NULL,
    [LastActivityDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_AnonymousUser_AnonymousUserID] PRIMARY KEY CLUSTERED ([AnonymousUserID] ASC)
);

