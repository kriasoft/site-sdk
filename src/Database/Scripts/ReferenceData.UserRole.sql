--------------------------------------------------------------------------------------------------------------------
-- Reference data for [Membership].[UserRole]
--------------------------------------------------------------------------------------------------------------------

--SET IDENTITY_INSERT [Membership].[UserRole] ON
--GO

MERGE INTO [Membership].[UserRole] AS Target
USING (VALUES
  (1, N'Administrator'),
  (2, N'Moderator')
)
AS Source ([RoleID], [Name])
ON Target.[RoleID] = Source.[RoleID]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET [Name] = Source.[Name]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([RoleID], [Name])
VALUES ([RoleID], [Name]);
-- delete rows that are in the target but not the source
-- WHEN NOT MATCHED BY SOURCE THEN 
-- DELETE
GO

--SET IDENTITY_INSERT [Membership].[UserRole] OFF
--GO
