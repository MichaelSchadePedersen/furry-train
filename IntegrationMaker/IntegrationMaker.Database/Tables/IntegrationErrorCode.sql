CREATE TABLE [dbo].[IntegrationErrorCode]
(
	[Id] BIGINT IDENTITY(1,1),
	[Number] BIGINT NOT NULL,
	[Name] NVARCHAR(500) NOT NULL,
	[UserfriendlyDescreption] NVARCHAR(max) NOT NULL,
	[TechnicalDescription] NVARCHAR(max) NOT NULL, 
    CONSTRAINT [PK_IntegrationErrorCodes] PRIMARY KEY (Number)
)
