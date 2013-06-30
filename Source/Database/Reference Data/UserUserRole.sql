MERGE INTO [Membership].[UserUserRole] AS Target
USING (VALUES

    (1, 1)

) AS Source ([UserID], [RoleID])
ON Target.[UserID] = Source.[UserID] AND Target.[RoleID] = Source.[RoleID]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([UserID], [RoleID])
VALUES ([UserID], [RoleID]);
GO