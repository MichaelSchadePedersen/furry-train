CREATE TABLE [dbo].[IntegrationType]
(
	[Id] BIGINT IDENTITY(1,1),
	[Name] NVARCHAR(500) NOT NULL,
	[CreatedUTC] DATETIME NOT NULL, 
    CONSTRAINT [PK_IntegrationTypes] PRIMARY KEY ([Id]),
)
