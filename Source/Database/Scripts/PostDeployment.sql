---------------------------------------------------------------------------------------------------
-- Import ReferenceData\UserRole.csv
---------------------------------------------------------------------------------------------------

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;

SELECT TOP(0) CAST([RoleID] AS TINYINT) AS [RoleID], [RoleName] INTO #Temp FROM [Membership].[UserRole];

BULK INSERT #Temp FROM '$(ReferenceDataDir)UserRole.csv'
WITH (FIRSTROW = 2, FIELDTERMINATOR = ' | ', ROWTERMINATOR = '\n', KEEPNULLS);
GO

MERGE INTO [Membership].[UserRole] AS Target
USING (SELECT [RoleID], [RoleName] FROM #Temp) AS Source
ON Target.[RoleID] = Source.[RoleID]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET [RoleName] = Source.[RoleName]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([RoleID], [RoleName])
VALUES ([RoleID], [RoleName]);
-- delete rows that are in the target but not the source
-- WHEN NOT MATCHED BY SOURCE THEN 
-- DELETE;
GO

IF OBJECT_ID('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp;
GO
