CREATE TABLE [dbo].[CompanyUser]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(1000) NOT NULL,
	[CompanyId] BIGINT NOT NULL, 
    CONSTRAINT [FK_CompanyUser_Company] FOREIGN KEY (CompanyId) References [Company]([Id])
	)
