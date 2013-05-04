CREATE TABLE [Membership].[Profile] (
    [UserID]               INT             NOT NULL,
    [PropertyNames]        NVARCHAR (MAX)  NOT NULL,
    [PropertyValueStrings] NVARCHAR (MAX)  NOT NULL,
    [PropertyValueBinary]  VARBINARY (MAX) NOT NULL,
    [LastUpdatedDate]      DATETIME        NOT NULL,
    CONSTRAINT [PK_Profile_UserID] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_Profile_User] FOREIGN KEY ([UserID]) REFERENCES [Membership].[User] ([UserID])
);

