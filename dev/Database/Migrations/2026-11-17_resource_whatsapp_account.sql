-- Adds the WHATSAPP_ACCOUNT resource so it can be assigned from the roles screen.
-- Permissions are resolved by resource.description, so the id is not relevant.

USE `buslogix`;

INSERT INTO `resource` (`description`)
SELECT 'WHATSAPP_ACCOUNT'
 WHERE NOT EXISTS (SELECT 1 FROM `resource` WHERE `description` = 'WHATSAPP_ACCOUNT');
