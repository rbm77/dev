-- Makes payment_request.receipt_reference optional.
-- A request without reference is stored with NULL, is never auto-matched and
-- is approved without a payment_receipt_reference row.

USE `buslogix`;

ALTER TABLE `payment_request`
  DROP CHECK `chk_payment_request_receipt_reference`;

ALTER TABLE `payment_request`
  MODIFY `receipt_reference` varchar(50) DEFAULT NULL,
  ADD CONSTRAINT `chk_payment_request_receipt_reference` CHECK (((`receipt_reference` is null) or (char_length(trim(`receipt_reference`)) > 0)));

/*!50003 DROP PROCEDURE IF EXISTS `approve_payment_request` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `approve_payment_request`(
    IN  p_company_id INT,
    IN  p_id INT,
    OUT p_new_payment_id BIGINT
)
BEGIN
    DECLARE v_date DATE;
    DECLARE v_amount DECIMAL(10,2);
    DECLARE v_student_id INT;
    DECLARE v_receipt_reference VARCHAR(50);
    DECLARE v_is_validated TINYINT DEFAULT NULL;
    DECLARE v_new_payment_id BIGINT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SET p_new_payment_id = 0;

    START TRANSACTION;

    SELECT `date`, amount, student_id, receipt_reference, is_validated
      INTO v_date, v_amount, v_student_id, v_receipt_reference, v_is_validated
      FROM `payment_request`
     WHERE company_id = p_company_id AND id = p_id
       FOR UPDATE;

    IF v_is_validated = 1 THEN
        SELECT IFNULL(MAX(id), 0) + 1 INTO v_new_payment_id
          FROM `payment`
         WHERE company_id = p_company_id
           FOR UPDATE;

        INSERT INTO `payment` (company_id, id, `date`, amount, student_id)
        VALUES (p_company_id, v_new_payment_id, v_date, v_amount, v_student_id);

        IF v_receipt_reference IS NOT NULL THEN
            INSERT INTO `payment_receipt_reference` (receipt_reference, company_id, payment_id)
            VALUES (v_receipt_reference, p_company_id, v_new_payment_id);
        END IF;

        DELETE FROM `payment_request`
         WHERE company_id = p_company_id AND id = p_id;

        SET p_new_payment_id = v_new_payment_id;
    END IF;

    COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `insert_payment_request` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `insert_payment_request`(
    IN p_company_id INT,
    IN p_date DATE,
    IN p_amount DECIMAL(10,2),
    IN p_student_id INT,
    IN p_receipt_reference VARCHAR(50)
)
BEGIN
    DECLARE v_id INT;
    DECLARE v_exists INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SET p_receipt_reference = NULLIF(TRIM(p_receipt_reference), '');

    IF p_receipt_reference IS NOT NULL THEN
        SELECT
            (SELECT COUNT(*) FROM `payment_receipt_reference` WHERE receipt_reference = p_receipt_reference)
          + (SELECT COUNT(*) FROM `payment_request`           WHERE receipt_reference = p_receipt_reference)
          INTO v_exists;
    END IF;

    IF v_exists > 0 THEN
        SELECT -1 AS new_id;
    ELSE
        START TRANSACTION;

        SELECT IFNULL(MAX(id), 0) + 1 INTO v_id
          FROM `payment_request`
         WHERE company_id = p_company_id
           FOR UPDATE;

        INSERT INTO `payment_request` (
            company_id, id, `date`, amount, student_id, receipt_reference, requested_at
        ) VALUES (
            p_company_id, v_id, p_date, p_amount, p_student_id, p_receipt_reference, NOW()
        );

        COMMIT;

        SELECT v_id AS new_id;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `match_payment_request_extraction` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `match_payment_request_extraction`(
    IN  p_company_id INT,
    IN  p_reference VARCHAR(50),
    OUT p_matched INT,
    OUT p_amount_mismatch INT,
    OUT p_payment_request_id INT,
    OUT p_new_payment_id BIGINT
)
BEGIN
    DECLARE v_pr_id INT DEFAULT NULL;
    DECLARE v_pr_amount DECIMAL(10,2) DEFAULT NULL;
    DECLARE v_mer_id BIGINT DEFAULT NULL;
    DECLARE v_mer_amount DECIMAL(10,2) DEFAULT NULL;
    DECLARE v_auto_approval_enabled INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SET p_matched = 0;
    SET p_amount_mismatch = 0;
    SET p_payment_request_id = NULL;
    SET p_new_payment_id = 0;

    IF p_reference IS NOT NULL THEN
        START TRANSACTION;

        SELECT id, amount INTO v_pr_id, v_pr_amount
          FROM `payment_request`
         WHERE company_id = p_company_id AND receipt_reference = p_reference
           FOR UPDATE;

        SELECT id, amount INTO v_mer_id, v_mer_amount
          FROM `message_extraction_result`
         WHERE company_id = p_company_id AND reference = p_reference
           FOR UPDATE;

        IF v_pr_id IS NOT NULL AND v_mer_id IS NOT NULL THEN
            IF v_pr_amount = v_mer_amount THEN
                DELETE FROM `message_extraction_result` WHERE id = v_mer_id;

                UPDATE `payment_request`
                   SET is_validated = 1
                 WHERE company_id = p_company_id AND id = v_pr_id;

                SET p_matched = 1;
                SET p_payment_request_id = v_pr_id;
            ELSE
                SET p_amount_mismatch = 1;
            END IF;
        END IF;

        COMMIT;
    END IF;

    IF p_matched = 1 THEN
        SELECT COUNT(*) INTO v_auto_approval_enabled
          FROM `company` c
         WHERE c.id = p_company_id
           AND c.auto_approval_enabled = 1
           AND c.is_active = 1;

        IF v_auto_approval_enabled > 0 THEN
            CALL approve_payment_request(p_company_id, v_pr_id, p_new_payment_id);
        END IF;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
