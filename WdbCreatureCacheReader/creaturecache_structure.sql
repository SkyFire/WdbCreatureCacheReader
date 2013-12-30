DROP TABLE IF EXISTS `creaturecache`;

CREATE TABLE `creaturecache`
(
	`locale`                    char(5) default NULL,
	`entry`                     INT UNSIGNED NOT NULL DEFAULT '0',

	`sub_name_len`              INT UNSIGNED NOT NULL DEFAULT '0',
	`unk_name_len`              INT UNSIGNED NOT NULL DEFAULT '0',
	`icon_name_len`             INT UNSIGNED NOT NULL DEFAULT '0',
	`racial_leader`             INT UNSIGNED NOT NULL DEFAULT '0',
	
	`male_name_len1`            INT UNSIGNED NOT NULL DEFAULT '0',
	`female_name_len1`          INT UNSIGNED NOT NULL DEFAULT '0',
	`male_name_len2`            INT UNSIGNED NOT NULL DEFAULT '0',
	`female_name_len2`          INT UNSIGNED NOT NULL DEFAULT '0',
	`male_name_len3`            INT UNSIGNED NOT NULL DEFAULT '0',
	`female_name_len3`          INT UNSIGNED NOT NULL DEFAULT '0',
	`male_name_len4`            INT UNSIGNED NOT NULL DEFAULT '0',
	`female_name_len4`          INT UNSIGNED NOT NULL DEFAULT '0',
	
	`male_name1`                TEXT,
	`female_name1`              TEXT,
	`male_name2`                TEXT,
	`female_name2`              TEXT,
	`male_name3`                TEXT,
	`female_name3`              TEXT,
	`male_name4`                TEXT,
	`female_name4`              TEXT,
	
	`type_flags`                INT UNSIGNED NOT NULL DEFAULT '0',
	`unk_541`                   INT NOT NULL DEFAULT '0',	-- flag???
	`type`                      INT NOT NULL DEFAULT '0',
	`family`                    INT NOT NULL DEFAULT '0',
	`rank`                      INT NOT NULL DEFAULT '0',
	
	`killcredit1`               INT NOT NULL DEFAULT '0',
	`killcredit2`               INT NOT NULL DEFAULT '0',
	
	`modelId1`                  INT NOT NULL DEFAULT '0',
	`modelId2`                  INT NOT NULL DEFAULT '0',
	`modelId3`                  INT NOT NULL DEFAULT '0',
	`modelId4`                  INT NOT NULL DEFAULT '0',
	
	`HealthModifier`            FLOAT NOT NULL DEFAULT '0',
    `PowerModifier`             FLOAT NOT NULL DEFAULT '0',
	
	`quest_item_count`          INT NOT NULL DEFAULT '0',
	`movementId`                INT NOT NULL DEFAULT '0',
	`unk543`                    INT NOT NULL DEFAULT '0',   -- addon version ??? 
	
	`subname`                   TEXT,
	`unk_text`                  TEXT,                       -- sub text
	`icon_name`                 TEXT,
	
	`QuestItem1`                INT UNSIGNED NOT NULL DEFAULT '0',
    `QuestItem2`                INT UNSIGNED NOT NULL DEFAULT '0',
    `QuestItem3`                INT UNSIGNED NOT NULL DEFAULT '0',
    `QuestItem4`                INT UNSIGNED NOT NULL DEFAULT '0',
    `QuestItem5`                INT UNSIGNED NOT NULL DEFAULT '0',
    `QuestItem6`                INT UNSIGNED NOT NULL DEFAULT '0',

	PRIMARY KEY (`locale`, `entry`)

) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Export of creaturecache';