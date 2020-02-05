CREATE TABLE "Snapshots" (
    "id"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "timestamp" INTEGER NOT NULL
);

CREATE TABLE "Realms" (
    "id"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "name"  TEXT NOT NULL,
    "slug"  TEXT NOT NULL
);

CREATE TABLE "SnapshotRealmDetails" (
    "id"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "snapshotId"    INTEGER NOT NULL,
    "realmId"   INTEGER NOT NULL,
    CONSTRAINT "FK_snapshotId" FOREIGN KEY("snapshotId") REFERENCES "Snapshots"("id"),
    CONSTRAINT "FK_realmId" FOREIGN KEY("realmId") REFERENCES "Realms"("id")
);

CREATE TABLE "Auctions" (
	"id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "snapshotId"    INTEGER NOT NULL,
	"itemId"	INTEGER NOT NULL,
	"ownerRealmId"	INTEGER,
	"bid"	INTEGER NOT NULL,
	"buyout"	INTEGER NOT NULL,
	"quantity"	INTEGER NOT NULL,
	"timeLeft"	INTEGER NOT NULL,
	"rand"	INTEGER NOT NULL,
	"seed"	INTEGER NOT NULL,
	"context"	INTEGER NOT NULL,
    CONSTRAINT "FK_snapshotId" FOREIGN KEY("snapshotId") REFERENCES "Snapshots"("id")
);

CREATE TABLE "BonusListDetails" (
    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "auctionEntryId"    INTEGER NOT NULL,
    "bonusListId"  INTEGER NOT NULL,
    CONSTRAINT "FK_auctionEntryId" FOREIGN KEY("auctionEntryId") REFERENCES "Auctions"("id")
);

CREATE TABLE "ModifierDetails" (
    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "auctionEntryId"    INTEGER NOT NULL,
    "type"  INTEGER NOT NULL,
    "value" INTEGER NOT NULL,
    CONSTRAINT "FK_auctionEntryId" FOREIGN KEY("auctionEntryId") REFERENCES "Auctions"("id")
);

CREATE TABLE "AdditionalFieldNameDetails" (
    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "name"  TEXT NOT NULL
);

CREATE TABLE "AdditionalFieldDetails" (
    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "auctionEntryId"    INTEGER NOT NULL,
    "nameId"    INTEGER NOT NULL,
    "value" INTEGER NOT NULL,
    CONSTRAINT "FK_auctionEntryId" FOREIGN KEY("auctionEntryId") REFERENCES "Auctions"("id"),
    CONSTRAINT "FK_nameId" FOREIGN KEY("nameId") REFERENCES "AdditionalFieldNameDetail"("id")
);