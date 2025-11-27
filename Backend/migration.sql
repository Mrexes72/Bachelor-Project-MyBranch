IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [Discriminator] nvarchar(21) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Categories] (
    [CategoryId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId])
);
GO

CREATE TABLE [Images] (
    [ImageId] int NOT NULL IDENTITY,
    [ImagePath] nvarchar(max) NOT NULL,
    [ImageName] nvarchar(100) NOT NULL,
    [IsCarouselImage] bit NOT NULL,
    CONSTRAINT [PK_Images] PRIMARY KEY ([ImageId])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Drinks] (
    [DrinkId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [BasePrice] decimal(18,2) NULL,
    [SalePrice] decimal(18,2) NOT NULL,
    [CreatedByUserId] nvarchar(450) NULL,
    [CategoryId] int NULL,
    [ImagePath] nvarchar(max) NULL,
    [TimesFavorite] int NULL,
    CONSTRAINT [PK_Drinks] PRIMARY KEY ([DrinkId]),
    CONSTRAINT [FK_Drinks_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Drinks_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId])
);
GO

CREATE TABLE [MenuItems] (
    [MenuItemId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [IsAvailable] bit NOT NULL,
    [CategoryId] int NULL,
    [ImagePath] nvarchar(max) NULL,
    CONSTRAINT [PK_MenuItems] PRIMARY KEY ([MenuItemId]),
    CONSTRAINT [FK_MenuItems_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Ingredients] (
    [IngredientId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [IsAvailable] bit NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [Color] nvarchar(max) NULL,
    [FillLevel] int NULL,
    [CategoryId] int NULL,
    [ImagePath] nvarchar(max) NULL,
    [DrinkId] int NULL,
    CONSTRAINT [PK_Ingredients] PRIMARY KEY ([IngredientId]),
    CONSTRAINT [FK_Ingredients_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]),
    CONSTRAINT [FK_Ingredients_Drinks_DrinkId] FOREIGN KEY ([DrinkId]) REFERENCES [Drinks] ([DrinkId])
);
GO

CREATE TABLE [UserFavoriteDrinks] (
    [ApplicationUserId] nvarchar(450) NOT NULL,
    [FavoriteDrinksDrinkId] int NOT NULL,
    CONSTRAINT [PK_UserFavoriteDrinks] PRIMARY KEY ([ApplicationUserId], [FavoriteDrinksDrinkId]),
    CONSTRAINT [FK_UserFavoriteDrinks_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserFavoriteDrinks_Drinks_FavoriteDrinksDrinkId] FOREIGN KEY ([FavoriteDrinksDrinkId]) REFERENCES [Drinks] ([DrinkId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Drinks_CategoryId] ON [Drinks] ([CategoryId]);
GO

CREATE INDEX [IX_Drinks_CreatedByUserId] ON [Drinks] ([CreatedByUserId]);
GO

CREATE INDEX [IX_Ingredients_CategoryId] ON [Ingredients] ([CategoryId]);
GO

CREATE INDEX [IX_Ingredients_DrinkId] ON [Ingredients] ([DrinkId]);
GO

CREATE INDEX [IX_MenuItems_CategoryId] ON [MenuItems] ([CategoryId]);
GO

CREATE INDEX [IX_UserFavoriteDrinks_FavoriteDrinksDrinkId] ON [UserFavoriteDrinks] ([FavoriteDrinksDrinkId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250407224732_InitialSqlServerMigration', N'8.0.8');
GO

COMMIT;
GO

