PRAGMA user_version=2;
CREATE TABLE `OrganizationRatings` (
	`Id`	INTEGER,
	`OrganizationName`	nvarchar,
	`Rating` INTEGER,
	PRIMARY KEY(`Id`)
);

CREATE UNIQUE INDEX `IX_OrganizationRatings_OrganizationName` ON `OrganizationRatings` (
	`OrganizationName`
);
