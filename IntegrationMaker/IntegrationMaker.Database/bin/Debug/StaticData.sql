/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO [dbo].[IntegrationType]
           (Name,CreatedUTC)
     VALUES
           ('economic','2018-03-27')


INSERT INTO [dbo].[EconomicAPP]
(Name, AppSecretToken,RequestURL,AppPublicToken)
VALUES ('FurryTrain','BziA4Tls2Bgp4STB3lDJH0ReTtoUwK8xMCHC1AXMwI81','https://secure.e-conomic.com/secure/api1/requestaccess.aspx?appPublicToken=aM4bqkvcBR4HSTvgdRKnevJs8Y05lMLyKFivF90Qlto1','aM4bqkvcBR4HSTvgdRKnevJs8Y05lMLyKFivF90Qlto1')