CREATE TABLE [dbo].[EconomicAPP]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] nvarchar(500) NOT NULL,
	[AppSecretToken] NVARCHAR(500) NOT NULL,
	[RequestURL] NVARCHAR(500) NOT NULL,
	[AppPublicToken] NVARCHAR(500) NOT NULL
)
