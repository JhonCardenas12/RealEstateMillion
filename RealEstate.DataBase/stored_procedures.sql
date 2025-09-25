USE RealEstateDb;
GO

CREATE PROCEDURE sp_CreateUser
    @IdUser UNIQUEIDENTIFIER OUTPUT,
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(256),
    @FullName NVARCHAR(200),
    @Role NVARCHAR(50)
AS
BEGIN TRY
    SET NOCOUNT ON;
    SET @IdUser = NEWID();
    INSERT INTO [AppUser](IdUser, Username, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES(@IdUser, @Username, @PasswordHash, @FullName, @Role, 1, GETUTCDATE());
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_CreateUser', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_GetUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN TRY
    SET NOCOUNT ON;
    SELECT * FROM AppUser WITH (NOLOCK) WHERE Username = @Username AND IsActive = 1;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_GetUserByUsername', ERROR_MESSAGE());
END CATCH;
GO


CREATE PROCEDURE sp_AddPropertyTrace
    @IdPropertyTrace UNIQUEIDENTIFIER OUTPUT,
    @IdProperty UNIQUEIDENTIFIER,
    @DateSale DATETIME,
    @Name NVARCHAR(200),
    @Value DECIMAL(18,2),
    @Tax DECIMAL(18,2),
    @TraceType NVARCHAR(100),
    @Notes NVARCHAR(MAX)
AS
BEGIN TRY
    SET NOCOUNT ON;
    SET @IdPropertyTrace = NEWID();
    INSERT INTO PropertyTrace (IdPropertyTrace, IdProperty, DateSale, Name, Value, Tax, TraceType, Notes)
    VALUES (@IdPropertyTrace, @IdProperty, @DateSale, @Name, @Value, @Tax, @TraceType, @Notes);
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_AddPropertyTrace', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_AddPropertyImage
    @IdPropertyImage UNIQUEIDENTIFIER OUTPUT,
    @IdProperty UNIQUEIDENTIFIER,
    @FileName NVARCHAR(500),
    @ContentType NVARCHAR(100),
    @Size BIGINT
AS
BEGIN TRY
    SET NOCOUNT ON;
    SET @IdPropertyImage = NEWID();
    INSERT INTO [PropertyImage](IdPropertyImage, IdProperty, FileName, ContentType, Size, Enabled, CreatedAt)
    VALUES(@IdPropertyImage, @IdProperty, @FileName, @ContentType, @Size, 1, GETUTCDATE());
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_AddPropertyImage', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_GetPropertyImagesByPropertyId
    @IdProperty UNIQUEIDENTIFIER
AS
BEGIN TRY
    SET NOCOUNT ON;
    SELECT * FROM [PropertyImage] WITH (NOLOCK) WHERE IdProperty = @IdProperty AND Enabled = 1 ORDER BY CreatedAt DESC;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_GetPropertyImagesByPropertyId', ERROR_MESSAGE());
END CATCH;
GO


CREATE PROCEDURE sp_CreateProperty
    @IdProperty UNIQUEIDENTIFIER OUTPUT,
    @Name NVARCHAR(200),
    @CodeInternal NVARCHAR(100),
    @Address NVARCHAR(500),
    @Price DECIMAL(18,2),
    @Year INT,
    @IdOwner UNIQUEIDENTIFIER,
    @Description NVARCHAR(MAX),
    @Bedrooms INT,
    @Bathrooms INT,
    @SquareMeters DECIMAL(10,2)
AS
BEGIN TRY
    SET NOCOUNT ON;
    SET @IdProperty = NEWID();
    INSERT INTO [Property](IdProperty, Name, CodeInternal, Address, Price, Year, IdOwner, Description, Bedrooms, Bathrooms, SquareMeters, IsActive, CreatedAt)
    VALUES(@IdProperty,@Name,@CodeInternal,@Address,@Price,@Year,@IdOwner,@Description,@Bedrooms,@Bathrooms,@SquareMeters,1,GETUTCDATE());
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_CreateProperty', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_GetPropertyById
    @IdProperty UNIQUEIDENTIFIER
AS
BEGIN TRY
    SET NOCOUNT ON;
    SELECT * FROM [Property] WITH (NOLOCK) WHERE IdProperty = @IdProperty AND IsActive = 1;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_GetPropertyById', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_GetPropertyDetailedById
    @IdProperty UNIQUEIDENTIFIER
AS
BEGIN TRY
    SET NOCOUNT ON;

    SELECT * FROM [Property] WITH (NOLOCK) WHERE IdProperty = @IdProperty AND IsActive = 1;
    SELECT * FROM [PropertyImage] WITH (NOLOCK) WHERE IdProperty = @IdProperty AND Enabled = 1;
    SELECT * FROM [PropertyTrace] WITH (NOLOCK) WHERE IdProperty = @IdProperty ORDER BY DateSale DESC;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_GetPropertyDetailedById', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_ListProperties
    @Name NVARCHAR(200) = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @IdOwner UNIQUEIDENTIFIER = NULL
AS
BEGIN TRY
    SET NOCOUNT ON;

    SELECT * FROM [Property] WITH (NOLOCK)
    WHERE IsActive = 1
      AND (@Name IS NULL OR Name LIKE '%' + @Name + '%')
      AND (@MinPrice IS NULL OR Price >= @MinPrice)
      AND (@MaxPrice IS NULL OR Price <= @MaxPrice)
      AND (@IdOwner IS NULL OR IdOwner = @IdOwner)
    ORDER BY CreatedAt DESC;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_ListProperties', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_UpdateProperty
    @IdProperty UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @Address NVARCHAR(500),
    @Price DECIMAL(18,2),
    @Description NVARCHAR(MAX),
    @Bedrooms INT,
    @Bathrooms INT,
    @SquareMeters DECIMAL(10,2)
AS
BEGIN TRY
    SET NOCOUNT ON;

    UPDATE [Property]
    SET Name=@Name, Address=@Address, Price=@Price, Description=@Description,
        Bedrooms=@Bedrooms, Bathrooms=@Bathrooms, SquareMeters=@SquareMeters,
        UpdatedAt = GETUTCDATE()
    WHERE IdProperty = @IdProperty;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_UpdateProperty', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_ChangePropertyPrice
    @IdProperty UNIQUEIDENTIFIER,
    @NewPrice DECIMAL(18,2),
    @Reason NVARCHAR(1000)
AS
BEGIN TRY
    SET NOCOUNT ON;

    UPDATE [Property] SET Price = @NewPrice, UpdatedAt = GETUTCDATE()
    WHERE IdProperty = @IdProperty;

    INSERT INTO PropertyTrace (IdPropertyTrace, IdProperty, DateSale, Name, Value, Tax, TraceType, Notes)
    VALUES (NEWID(), @IdProperty, GETUTCDATE(), 'PriceChange', @NewPrice, 0, 'PriceChange', @Reason);
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_ChangePropertyPrice', ERROR_MESSAGE());
END CATCH;
GO


CREATE PROCEDURE sp_CreateOwner
    @IdOwner UNIQUEIDENTIFIER OUTPUT,
    @Name NVARCHAR(200),
    @Address NVARCHAR(500),
    @ContactEmail NVARCHAR(200),
	@PhotoFileName NVARCHAR(500),
	@Birthday DATETIME,
    @Phone NVARCHAR(50)
AS
BEGIN TRY
    SET NOCOUNT ON;
    SET @IdOwner = NEWID();
    INSERT INTO [Owner](IdOwner, Name, Address, ContactEmail, PhotoFileName, Birthday, Phone, IsActive, CreatedAt)
    VALUES(@IdOwner, @Name, @Address, @ContactEmail, @PhotoFileName,@Birthday, @Phone, 1, GETUTCDATE());
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_CreateOwner', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_GetOwnerById
    @IdOwner UNIQUEIDENTIFIER
AS
BEGIN TRY
    SET NOCOUNT ON;
    SELECT * FROM [Owner] WITH (NOLOCK) WHERE IdOwner = @IdOwner AND IsActive = 1;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_GetOwnerById', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_ListOwners
AS
BEGIN TRY
    SET NOCOUNT ON;
    SELECT * FROM [Owner] WITH (NOLOCK) WHERE IsActive = 1;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_ListOwners', ERROR_MESSAGE());
END CATCH;
GO


CREATE PROCEDURE sp_UpdateOwner
    @IdOwner UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @Address NVARCHAR(500),
    @ContactEmail NVARCHAR(200),
	@PhotoFileName NVARCHAR(500),
	@Birthday DATETIME,
    @Phone NVARCHAR(50)
AS
BEGIN TRY
    SET NOCOUNT ON;
    UPDATE [Owner] SET Name=@Name, Address=@Address, ContactEmail=@ContactEmail,PhotoFileName= @PhotoFileName, Birthday= @Birthday, Phone=@Phone, UpdatedAt = GETUTCDATE()
    WHERE IdOwner=@IdOwner;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_UpdateOwner', ERROR_MESSAGE());
END CATCH;
GO

CREATE PROCEDURE sp_DeleteOwner
    @IdOwner UNIQUEIDENTIFIER
AS
BEGIN TRY
    SET NOCOUNT ON;
    UPDATE [Owner] SET IsActive = 0, UpdatedAt = GETUTCDATE() WHERE IdOwner = @IdOwner;
END TRY
BEGIN CATCH
    INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
    VALUES ('sp_DeleteOwner', ERROR_MESSAGE());
END CATCH;
GO


CREATE PROCEDURE st_UpdateProperty
    @IdProperty UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @CodeInternal NVARCHAR(50),
    @Address NVARCHAR(300),
    @Price DECIMAL(18,2),
    @Year INT,
    @IdOwner UNIQUEIDENTIFIER,
    @Description NVARCHAR(500),
    @Bedrooms INT,
    @Bathrooms INT,
    @SquareMeters DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Property
    SET Name = @Name,
        CodeInternal = @CodeInternal,
        Address = @Address,
        Price = @Price,
        Year = @Year,
        IdOwner = @IdOwner,
        Description = @Description,
        Bedrooms = @Bedrooms,
        Bathrooms = @Bathrooms,
        SquareMeters = @SquareMeters
    WHERE IdProperty = @IdProperty;
END;
GO

CREATE PROCEDURE sp_GetPropertyTraceById
    @IdPropertyTrace UNIQUEIDENTIFIER
AS
BEGIN
    BEGIN TRY
        SET NOCOUNT ON;
        
        SELECT 
            IdPropertyTrace,
            IdProperty,
            DateSale,
            Name,
            Value,
            Tax,
            TraceType,
            Notes
        FROM PropertyTrace 
        WHERE IdPropertyTrace = @IdPropertyTrace;
        
    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLogForStoreProcedure (StoreProcedure, ErrorMessage)
        VALUES ('sp_GetPropertyTraceById', ERROR_MESSAGE());
        THROW;
    END CATCH;
END;
GO