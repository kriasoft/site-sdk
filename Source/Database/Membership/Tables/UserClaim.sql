CREATE TABLE [Membership].[UserClaim] (
    [ClaimID]    UNIQUEIDENTIFIER CONSTRAINT [DF_UserClaim_ClaimID] DEFAULT (NEWID()) NOT NULL,
    [UserID]     INT              NOT NULL,
    [ClaimType]  NVARCHAR (MAX)   NULL,
    [ClaimValue] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_UserClaim_Key] PRIMARY KEY CLUSTERED ([ClaimID] ASC), 
    CONSTRAINT [FK_UserClaim_User] FOREIGN KEY ([UserID]) REFERENCES [Membership].[User] ([UserID])
);