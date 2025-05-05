--
-- PostgreSQL database dump
--

-- Dumped from database version 15.8
-- Dumped by pg_dump version 16.4

-- Started on 2025-05-05 01:39:41

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 298 (class 1255 OID 82500)
-- Name: approve_all_pending_reviews(); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.approve_all_pending_reviews()
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Одобряем все отзывы о тренерах
    UPDATE trainer_reviews
    SET moderated = TRUE
    WHERE moderated = FALSE;

    -- Одобряем все отзывы о занятиях
    UPDATE class_reviews
    SET moderated = TRUE
    WHERE moderated = FALSE;
END;
$$;


ALTER PROCEDURE public.approve_all_pending_reviews() OWNER TO postgres;

--
-- TOC entry 295 (class 1255 OID 82498)
-- Name: clear_unpaid_services(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.clear_unpaid_services(IN in_client_id integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM services_payments
    WHERE client_id = in_client_id
    AND payment_date IS NULL;
END;
$$;


ALTER PROCEDURE public.clear_unpaid_services(IN in_client_id integer) OWNER TO postgres;

--
-- TOC entry 280 (class 1255 OID 82493)
-- Name: count_visited_classes(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.count_visited_classes(client_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN (
        SELECT COUNT(*)
        FROM classvisits cv
        JOIN clientmembership cm ON cv.client_membership_id = cm.client_membership_id
        WHERE cm.client_id = client_id AND cv.visited = true
    );
END;
$$;


ALTER FUNCTION public.count_visited_classes(client_id integer) OWNER TO postgres;

--
-- TOC entry 283 (class 1255 OID 82496)
-- Name: deduct_balance(integer, numeric, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.deduct_balance(IN in_client_id integer, IN in_amount numeric, IN in_description text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE client
    SET balance = balance - in_amount
    WHERE client_id = in_client_id;

    INSERT INTO clienttransactions (client_id, operation_description, payment_way, amount, transaction_type, transaction_date)
    VALUES (in_client_id, in_description, 'С баланса', -in_amount, 'списание', CURRENT_DATE);
END;
$$;


ALTER PROCEDURE public.deduct_balance(IN in_client_id integer, IN in_amount numeric, IN in_description text) OWNER TO postgres;

--
-- TOC entry 297 (class 1255 OID 82499)
-- Name: delete_equipment_completely(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.delete_equipment_completely(IN in_equipment_id integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM hall_equipment
    WHERE equipment_id = in_equipment_id;

    DELETE FROM equipment
    WHERE equipment_id = in_equipment_id;
END;
$$;


ALTER PROCEDURE public.delete_equipment_completely(IN in_equipment_id integer) OWNER TO postgres;

--
-- TOC entry 272 (class 1255 OID 65949)
-- Name: delete_user_when_client_deleted(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.delete_user_when_client_deleted() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Проверяем, что user_id не NULL
    IF OLD.user_id IS NOT NULL THEN
        -- Удаляем пользователя только если он не связан с тренером
        IF NOT EXISTS (
            SELECT 1 FROM trainer 
            WHERE user_id = OLD.user_id
        ) THEN
            DELETE FROM users 
            WHERE user_id = OLD.user_id;
        END IF;
    END IF;
    
    RETURN OLD;
END;
$$;


ALTER FUNCTION public.delete_user_when_client_deleted() OWNER TO postgres;

--
-- TOC entry 273 (class 1255 OID 65951)
-- Name: delete_user_when_trainer_deleted(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.delete_user_when_trainer_deleted() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Проверяем, что user_id не NULL
    IF OLD.user_id IS NOT NULL THEN
        -- Удаляем пользователя только если он не связан с тренером
        IF NOT EXISTS (
            SELECT 1 FROM client 
            WHERE user_id = OLD.user_id
        ) THEN
            DELETE FROM users 
            WHERE user_id = OLD.user_id;
        END IF;
    END IF;
    
    RETURN OLD;
END;
$$;


ALTER FUNCTION public.delete_user_when_trainer_deleted() OWNER TO postgres;

--
-- TOC entry 276 (class 1255 OID 82485)
-- Name: empty_review_check(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.empty_review_check() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.review_content IS NULL OR TRIM(NEW.review_content) = '' THEN
        NEW.review_content := 'Отзыв без подробностей';
    END IF;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.empty_review_check() OWNER TO postgres;

--
-- TOC entry 279 (class 1255 OID 82492)
-- Name: get_trainer_avg_rating(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_trainer_avg_rating(trainer_id integer) RETURNS double precision
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN (
        SELECT AVG(review_grade)
        FROM trainer_reviews
        WHERE trainer_id = trainer_id AND moderated = true
    );
END;
$$;


ALTER FUNCTION public.get_trainer_avg_rating(trainer_id integer) OWNER TO postgres;

--
-- TOC entry 274 (class 1255 OID 65956)
-- Name: insert_user_on_client(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.insert_user_on_client() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    new_user_id INT;
BEGIN
    RAISE NOTICE 'Триггер insert_user_on_client сработал для клиента: % %', NEW.last_name, NEW.first_name;

    -- Вставляем пустого пользователя
    INSERT INTO users (login, password, role_id)
    VALUES ('', '', NULL)
    RETURNING user_id INTO new_user_id;

    RAISE NOTICE 'Создан пользователь с user_id = %', new_user_id;

    -- Присваиваем user_id клиенту
    NEW.user_id := new_user_id;

    RAISE NOTICE 'user_id присвоен клиенту: %', NEW.user_id;

    RETURN NEW;
END;
$$;


ALTER FUNCTION public.insert_user_on_client() OWNER TO postgres;

--
-- TOC entry 277 (class 1255 OID 82489)
-- Name: limit_equipment_quantity(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.limit_equipment_quantity() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.quantity > 50 THEN
        NEW.quantity := 50;
    END IF;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.limit_equipment_quantity() OWNER TO postgres;

--
-- TOC entry 282 (class 1255 OID 82495)
-- Name: locker_days_left(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.locker_days_left(client_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    end_date DATE;
BEGIN
    SELECT rl.end_date
    INTO end_date
    FROM rented_lockers rl
    JOIN clientmembership cm ON rl.client_membership_id = cm.client_membership_id
    WHERE cm.client_id = client_id
    ORDER BY rl.end_date DESC
    LIMIT 1;

    IF end_date IS NULL THEN
        RETURN 0;
    ELSE
        RETURN GREATEST(0, end_date - CURRENT_DATE);
    END IF;
END;
$$;


ALTER FUNCTION public.locker_days_left(client_id integer) OWNER TO postgres;

--
-- TOC entry 278 (class 1255 OID 82491)
-- Name: membership_days_left(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.membership_days_left(client_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    end_date DATE;
BEGIN
    SELECT end_date INTO end_date
    FROM clientmembership
    WHERE client_id = client_id
    ORDER BY end_date DESC
    LIMIT 1;

    IF end_date IS NULL THEN
        RETURN 0;
    ELSE
        RETURN GREATEST(0, end_date - CURRENT_DATE);
    END IF;
END;
$$;


ALTER FUNCTION public.membership_days_left(client_id integer) OWNER TO postgres;

--
-- TOC entry 288 (class 1255 OID 82497)
-- Name: refund_to_balance(integer, numeric, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.refund_to_balance(IN in_client_id integer, IN in_amount numeric, IN in_description text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE client
    SET balance = balance + in_amount
    WHERE client_id = in_client_id;

    INSERT INTO clienttransactions (client_id, operation_description, payment_way, amount, transaction_type, transaction_date)
    VALUES (in_client_id, in_description, 'На баланс', in_amount, 'возврат', CURRENT_DATE);
END;
$$;


ALTER PROCEDURE public.refund_to_balance(IN in_client_id integer, IN in_amount numeric, IN in_description text) OWNER TO postgres;

--
-- TOC entry 275 (class 1255 OID 82483)
-- Name: set_maintenance_date(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.set_maintenance_date() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.last_maintenance_date IS NULL THEN
        NEW.last_maintenance_date := NEW.delivery_date;
    END IF;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.set_maintenance_date() OWNER TO postgres;

--
-- TOC entry 281 (class 1255 OID 82494)
-- Name: unpaid_services_sum(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.unpaid_services_sum(client_id integer) RETURNS numeric
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN (
        SELECT COALESCE(SUM(price), 0)
        FROM services_payments
        WHERE client_id = client_id AND payment_date IS NULL
    );
END;
$$;


ALTER FUNCTION public.unpaid_services_sum(client_id integer) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 214 (class 1259 OID 65675)
-- Name: class; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.class (
    class_id integer NOT NULL,
    work_schedule_id integer NOT NULL,
    start_time time without time zone NOT NULL,
    end_time time without time zone NOT NULL,
    hall_id integer NOT NULL,
    class_type_id integer NOT NULL,
    people_quantity integer NOT NULL,
    price double precision,
    class_info_id integer,
    trainer_checked boolean DEFAULT false NOT NULL
);


ALTER TABLE public.class OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 65680)
-- Name: class_class_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.class_class_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.class_class_id_seq OWNER TO postgres;

--
-- TOC entry 3630 (class 0 OID 0)
-- Dependencies: 215
-- Name: class_class_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.class_class_id_seq OWNED BY public.class.class_id;


--
-- TOC entry 247 (class 1259 OID 66001)
-- Name: class_info; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.class_info (
    class_info_id integer NOT NULL,
    class_name character varying(100) NOT NULL,
    description text
);


ALTER TABLE public.class_info OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 66000)
-- Name: class_info_class_info_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.class_info_class_info_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.class_info_class_info_id_seq OWNER TO postgres;

--
-- TOC entry 3631 (class 0 OID 0)
-- Dependencies: 246
-- Name: class_info_class_info_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.class_info_class_info_id_seq OWNED BY public.class_info.class_info_id;


--
-- TOC entry 261 (class 1259 OID 74165)
-- Name: class_payments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.class_payments (
    class_payment_id integer NOT NULL,
    class_id integer,
    payment_date date,
    price double precision,
    client_id integer
);


ALTER TABLE public.class_payments OWNER TO postgres;

--
-- TOC entry 260 (class 1259 OID 74164)
-- Name: class_payments_class_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.class_payments_class_payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.class_payments_class_payment_id_seq OWNER TO postgres;

--
-- TOC entry 3632 (class 0 OID 0)
-- Dependencies: 260
-- Name: class_payments_class_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.class_payments_class_payment_id_seq OWNED BY public.class_payments.class_payment_id;


--
-- TOC entry 245 (class 1259 OID 65980)
-- Name: class_reviews; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.class_reviews (
    review_id integer NOT NULL,
    class_info_id integer,
    author_id integer,
    review_grade integer,
    review_content text,
    moderated boolean DEFAULT false,
    CONSTRAINT reviews_review_grade_check CHECK (((review_grade >= 1) AND (review_grade <= 5)))
);


ALTER TABLE public.class_reviews OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 65681)
-- Name: classtype; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.classtype (
    class_type_id integer NOT NULL,
    class_type_name character varying(50) NOT NULL
);


ALTER TABLE public.classtype OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 65684)
-- Name: classtype_class_type_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.classtype_class_type_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.classtype_class_type_id_seq OWNER TO postgres;

--
-- TOC entry 3633 (class 0 OID 0)
-- Dependencies: 217
-- Name: classtype_class_type_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.classtype_class_type_id_seq OWNED BY public.classtype.class_type_id;


--
-- TOC entry 218 (class 1259 OID 65685)
-- Name: classvisits; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.classvisits (
    visit_class_id integer NOT NULL,
    class_id integer NOT NULL,
    client_membership_id integer NOT NULL,
    visited boolean DEFAULT false
);


ALTER TABLE public.classvisits OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 65688)
-- Name: classvisits_visit_class_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.classvisits_visit_class_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.classvisits_visit_class_id_seq OWNER TO postgres;

--
-- TOC entry 3634 (class 0 OID 0)
-- Dependencies: 219
-- Name: classvisits_visit_class_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.classvisits_visit_class_id_seq OWNED BY public.classvisits.visit_class_id;


--
-- TOC entry 220 (class 1259 OID 65689)
-- Name: client; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.client (
    client_id integer NOT NULL,
    last_name character varying(50) NOT NULL,
    first_name character varying(50) NOT NULL,
    patronymic character varying(50),
    gender_id character(2) NOT NULL,
    birth_date date NOT NULL,
    passport_series character(4) NOT NULL,
    passport_number character(6) NOT NULL,
    phone_number character(15) NOT NULL,
    user_id integer,
    email character varying(256) NOT NULL,
    passport_kem_vidan character varying(250) NOT NULL,
    passport_kogda_vidan date NOT NULL,
    add_author_name boolean DEFAULT true,
    balance numeric(10,2) DEFAULT 0.00 NOT NULL,
    CONSTRAINT client_birth_date_check1 CHECK ((birth_date <= (CURRENT_DATE - '14 years'::interval)))
);


ALTER TABLE public.client OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 65692)
-- Name: client_client_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.client_client_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.client_client_id_seq OWNER TO postgres;

--
-- TOC entry 3635 (class 0 OID 0)
-- Dependencies: 221
-- Name: client_client_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.client_client_id_seq OWNED BY public.client.client_id;


--
-- TOC entry 271 (class 1259 OID 82468)
-- Name: client_transaction; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.client_transaction (
    transaction_id integer NOT NULL,
    client_id integer NOT NULL,
    operation_description text,
    payment_way character varying(50),
    amount numeric(10,2) NOT NULL,
    transaction_type character varying(20),
    transaction_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.client_transaction OWNER TO postgres;

--
-- TOC entry 270 (class 1259 OID 82467)
-- Name: client_transaction_transaction_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.client_transaction_transaction_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.client_transaction_transaction_id_seq OWNER TO postgres;

--
-- TOC entry 3636 (class 0 OID 0)
-- Dependencies: 270
-- Name: client_transaction_transaction_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.client_transaction_transaction_id_seq OWNED BY public.client_transaction.transaction_id;


--
-- TOC entry 222 (class 1259 OID 65693)
-- Name: clientmembership; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.clientmembership (
    client_membership_id integer NOT NULL,
    client_id integer NOT NULL,
    membership_id integer NOT NULL,
    start_date date DEFAULT CURRENT_DATE NOT NULL,
    end_date date NOT NULL
);


ALTER TABLE public.clientmembership OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 65697)
-- Name: clientmembership_client_membership_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.clientmembership_client_membership_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.clientmembership_client_membership_id_seq OWNER TO postgres;

--
-- TOC entry 3637 (class 0 OID 0)
-- Dependencies: 223
-- Name: clientmembership_client_membership_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.clientmembership_client_membership_id_seq OWNED BY public.clientmembership.client_membership_id;


--
-- TOC entry 224 (class 1259 OID 65698)
-- Name: equipment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.equipment (
    equipment_id integer NOT NULL,
    equipment_name character varying(100) NOT NULL,
    equipment_condition_id integer NOT NULL,
    delivery_date date NOT NULL,
    last_maintenance_date date NOT NULL,
    quantity integer NOT NULL
);


ALTER TABLE public.equipment OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 65702)
-- Name: equipment_equipment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.equipment_equipment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.equipment_equipment_id_seq OWNER TO postgres;

--
-- TOC entry 3638 (class 0 OID 0)
-- Dependencies: 225
-- Name: equipment_equipment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.equipment_equipment_id_seq OWNED BY public.equipment.equipment_id;


--
-- TOC entry 226 (class 1259 OID 65703)
-- Name: equipmentcondition; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.equipmentcondition (
    equipment_condition_id integer NOT NULL,
    equipment_condition_description character varying(50) NOT NULL
);


ALTER TABLE public.equipmentcondition OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 65706)
-- Name: equipmentcondition_equipment_condition_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.equipmentcondition_equipment_condition_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.equipmentcondition_equipment_condition_id_seq OWNER TO postgres;

--
-- TOC entry 3639 (class 0 OID 0)
-- Dependencies: 227
-- Name: equipmentcondition_equipment_condition_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.equipmentcondition_equipment_condition_id_seq OWNED BY public.equipmentcondition.equipment_condition_id;


--
-- TOC entry 228 (class 1259 OID 65707)
-- Name: gender; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.gender (
    gender_id character(2) NOT NULL,
    gender_name character varying(10) NOT NULL
);


ALTER TABLE public.gender OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 65710)
-- Name: hall; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.hall (
    hall_id integer NOT NULL,
    hall_name character varying(100) NOT NULL,
    capacity integer NOT NULL,
    description text,
    area double precision NOT NULL
);


ALTER TABLE public.hall OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 65715)
-- Name: hall_hall_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.hall_hall_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.hall_hall_id_seq OWNER TO postgres;

--
-- TOC entry 3640 (class 0 OID 0)
-- Dependencies: 230
-- Name: hall_hall_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.hall_hall_id_seq OWNED BY public.hall.hall_id;


--
-- TOC entry 231 (class 1259 OID 65716)
-- Name: hallequipment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.hallequipment (
    equipment_id integer NOT NULL,
    hall_id integer NOT NULL,
    quantity integer NOT NULL
);


ALTER TABLE public.hallequipment OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 66060)
-- Name: lockers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.lockers (
    locker_id integer NOT NULL,
    locker_zone character varying(50),
    month_price double precision
);


ALTER TABLE public.lockers OWNER TO postgres;

--
-- TOC entry 250 (class 1259 OID 66059)
-- Name: lockers_locker_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.lockers_locker_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.lockers_locker_id_seq OWNER TO postgres;

--
-- TOC entry 3641 (class 0 OID 0)
-- Dependencies: 250
-- Name: lockers_locker_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.lockers_locker_id_seq OWNED BY public.lockers.locker_id;


--
-- TOC entry 232 (class 1259 OID 65719)
-- Name: membership; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.membership (
    membership_id integer NOT NULL,
    membership_name character varying(100) NOT NULL,
    price integer NOT NULL,
    membership_type_id integer NOT NULL,
    membership_description text
);


ALTER TABLE public.membership OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 65722)
-- Name: membership_membership_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.membership_membership_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.membership_membership_id_seq OWNER TO postgres;

--
-- TOC entry 3642 (class 0 OID 0)
-- Dependencies: 233
-- Name: membership_membership_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.membership_membership_id_seq OWNED BY public.membership.membership_id;


--
-- TOC entry 265 (class 1259 OID 74199)
-- Name: membership_payments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.membership_payments (
    membership_payment_id integer NOT NULL,
    membership_id integer,
    payment_date date,
    price double precision,
    client_id integer
);


ALTER TABLE public.membership_payments OWNER TO postgres;

--
-- TOC entry 264 (class 1259 OID 74198)
-- Name: membership_payments_membership_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.membership_payments_membership_payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.membership_payments_membership_payment_id_seq OWNER TO postgres;

--
-- TOC entry 3643 (class 0 OID 0)
-- Dependencies: 264
-- Name: membership_payments_membership_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.membership_payments_membership_payment_id_seq OWNED BY public.membership_payments.membership_payment_id;


--
-- TOC entry 259 (class 1259 OID 66107)
-- Name: membership_services; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.membership_services (
    membershipservice_id integer NOT NULL,
    client_membership_id integer,
    service_id integer,
    provision_date date
);


ALTER TABLE public.membership_services OWNER TO postgres;

--
-- TOC entry 258 (class 1259 OID 66106)
-- Name: membership_services_membershipservice_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.membership_services_membershipservice_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.membership_services_membershipservice_id_seq OWNER TO postgres;

--
-- TOC entry 3644 (class 0 OID 0)
-- Dependencies: 258
-- Name: membership_services_membershipservice_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.membership_services_membershipservice_id_seq OWNED BY public.membership_services.membershipservice_id;


--
-- TOC entry 234 (class 1259 OID 65727)
-- Name: membershiptype; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.membershiptype (
    membership_type_id integer NOT NULL,
    membership_type_name character varying(50) NOT NULL,
    duration_months integer
);


ALTER TABLE public.membershiptype OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 65730)
-- Name: membershiptype_membership_type_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.membershiptype_membership_type_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.membershiptype_membership_type_id_seq OWNER TO postgres;

--
-- TOC entry 3645 (class 0 OID 0)
-- Dependencies: 235
-- Name: membershiptype_membership_type_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.membershiptype_membership_type_id_seq OWNED BY public.membershiptype.membership_type_id;


--
-- TOC entry 253 (class 1259 OID 66067)
-- Name: rented_lockers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rented_lockers (
    rent_locker_id integer NOT NULL,
    locker_id integer,
    client_membership_id integer,
    start_date date,
    end_date date,
    rent_price integer,
    payment_date date
);


ALTER TABLE public.rented_lockers OWNER TO postgres;

--
-- TOC entry 252 (class 1259 OID 66066)
-- Name: rented_lockers_rent_locker_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.rented_lockers_rent_locker_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.rented_lockers_rent_locker_id_seq OWNER TO postgres;

--
-- TOC entry 3646 (class 0 OID 0)
-- Dependencies: 252
-- Name: rented_lockers_rent_locker_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.rented_lockers_rent_locker_id_seq OWNED BY public.rented_lockers.rent_locker_id;


--
-- TOC entry 244 (class 1259 OID 65979)
-- Name: reviews_review_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.reviews_review_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.reviews_review_id_seq OWNER TO postgres;

--
-- TOC entry 3647 (class 0 OID 0)
-- Dependencies: 244
-- Name: reviews_review_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.reviews_review_id_seq OWNED BY public.class_reviews.review_id;


--
-- TOC entry 236 (class 1259 OID 65731)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    role_id integer NOT NULL,
    role_name character varying(255) NOT NULL
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 65734)
-- Name: roles_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roles_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roles_role_id_seq OWNER TO postgres;

--
-- TOC entry 3648 (class 0 OID 0)
-- Dependencies: 237
-- Name: roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roles_role_id_seq OWNED BY public.roles.role_id;


--
-- TOC entry 255 (class 1259 OID 66085)
-- Name: service_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.service_type (
    service_type_id integer NOT NULL,
    type_name character varying(100) NOT NULL
);


ALTER TABLE public.service_type OWNER TO postgres;

--
-- TOC entry 254 (class 1259 OID 66084)
-- Name: service_type_service_type_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.service_type_service_type_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.service_type_service_type_id_seq OWNER TO postgres;

--
-- TOC entry 3649 (class 0 OID 0)
-- Dependencies: 254
-- Name: service_type_service_type_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.service_type_service_type_id_seq OWNED BY public.service_type.service_type_id;


--
-- TOC entry 257 (class 1259 OID 66092)
-- Name: services; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.services (
    service_id integer NOT NULL,
    service_type_id integer,
    service_name character varying(100) NOT NULL,
    description text,
    price double precision,
    free_usage_limit integer DEFAULT 0
);


ALTER TABLE public.services OWNER TO postgres;

--
-- TOC entry 263 (class 1259 OID 74182)
-- Name: services_payments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.services_payments (
    service_payment_id integer NOT NULL,
    service_id integer,
    payment_date date,
    price double precision,
    client_id integer
);


ALTER TABLE public.services_payments OWNER TO postgres;

--
-- TOC entry 262 (class 1259 OID 74181)
-- Name: services_payments_service_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.services_payments_service_payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.services_payments_service_payment_id_seq OWNER TO postgres;

--
-- TOC entry 3650 (class 0 OID 0)
-- Dependencies: 262
-- Name: services_payments_service_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.services_payments_service_payment_id_seq OWNED BY public.services_payments.service_payment_id;


--
-- TOC entry 256 (class 1259 OID 66091)
-- Name: services_service_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.services_service_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.services_service_id_seq OWNER TO postgres;

--
-- TOC entry 3651 (class 0 OID 0)
-- Dependencies: 256
-- Name: services_service_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.services_service_id_seq OWNED BY public.services.service_id;


--
-- TOC entry 238 (class 1259 OID 65741)
-- Name: specialization; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.specialization (
    specialization_id integer NOT NULL,
    specialization_name character varying(100) NOT NULL,
    achievements character varying(200),
    education character varying(200)
);


ALTER TABLE public.specialization OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 65746)
-- Name: specialization_specialization_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.specialization_specialization_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.specialization_specialization_id_seq OWNER TO postgres;

--
-- TOC entry 3652 (class 0 OID 0)
-- Dependencies: 239
-- Name: specialization_specialization_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.specialization_specialization_id_seq OWNED BY public.specialization.specialization_id;


--
-- TOC entry 267 (class 1259 OID 82390)
-- Name: trainer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.trainer (
    trainer_id integer NOT NULL,
    photo text,
    last_name character varying(50) NOT NULL,
    first_name character varying(50) NOT NULL,
    patronymic character varying(50),
    gender_id character(2) NOT NULL,
    birth_date date NOT NULL,
    passport_number character(6) NOT NULL,
    passport_series character(4) NOT NULL,
    phone_number character varying(15) NOT NULL,
    specialization_id integer,
    user_id integer,
    email character varying(256) NOT NULL,
    passport_kogda_vidan date NOT NULL,
    passport_kem_vidan character varying(250) NOT NULL,
    individual_price double precision,
    date_of_employment date,
    CONSTRAINT trainer_birth_date_check CHECK ((birth_date <= (CURRENT_DATE - '18 years'::interval)))
);


ALTER TABLE public.trainer OWNER TO postgres;

--
-- TOC entry 249 (class 1259 OID 66015)
-- Name: trainer_reviews; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.trainer_reviews (
    trainer_review_id integer NOT NULL,
    trainer_id integer,
    author_id integer,
    review_grade integer,
    review_content text,
    moderated boolean DEFAULT false,
    CONSTRAINT trainer_reviews_review_grade_check CHECK (((review_grade >= 1) AND (review_grade <= 5)))
);


ALTER TABLE public.trainer_reviews OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 66014)
-- Name: trainer_reviews_trainer_review_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.trainer_reviews_trainer_review_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.trainer_reviews_trainer_review_id_seq OWNER TO postgres;

--
-- TOC entry 3653 (class 0 OID 0)
-- Dependencies: 248
-- Name: trainer_reviews_trainer_review_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.trainer_reviews_trainer_review_id_seq OWNED BY public.trainer_reviews.trainer_review_id;


--
-- TOC entry 266 (class 1259 OID 82389)
-- Name: trainer_trainer_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.trainer_trainer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.trainer_trainer_id_seq OWNER TO postgres;

--
-- TOC entry 3654 (class 0 OID 0)
-- Dependencies: 266
-- Name: trainer_trainer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.trainer_trainer_id_seq OWNED BY public.trainer.trainer_id;


--
-- TOC entry 269 (class 1259 OID 82445)
-- Name: training_plan; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.training_plan (
    training_plan_id integer NOT NULL,
    client_id integer NOT NULL,
    trainer_id integer NOT NULL,
    plan text NOT NULL,
    created_at timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.training_plan OWNER TO postgres;

--
-- TOC entry 268 (class 1259 OID 82444)
-- Name: training_plan_training_plan_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.training_plan_training_plan_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.training_plan_training_plan_id_seq OWNER TO postgres;

--
-- TOC entry 3655 (class 0 OID 0)
-- Dependencies: 268
-- Name: training_plan_training_plan_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.training_plan_training_plan_id_seq OWNED BY public.training_plan.training_plan_id;


--
-- TOC entry 240 (class 1259 OID 65753)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    login character varying(255),
    password character varying(255),
    role_id integer
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 65758)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 3656 (class 0 OID 0)
-- Dependencies: 241
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 242 (class 1259 OID 65759)
-- Name: workschedule; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.workschedule (
    work_schedule_id integer NOT NULL,
    trainer_id integer NOT NULL,
    work_date date NOT NULL,
    start_time time without time zone NOT NULL,
    end_time time without time zone NOT NULL
);


ALTER TABLE public.workschedule OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 65762)
-- Name: workschedule_work_schedule_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.workschedule_work_schedule_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.workschedule_work_schedule_id_seq OWNER TO postgres;

--
-- TOC entry 3657 (class 0 OID 0)
-- Dependencies: 243
-- Name: workschedule_work_schedule_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.workschedule_work_schedule_id_seq OWNED BY public.workschedule.work_schedule_id;


--
-- TOC entry 3332 (class 2604 OID 65763)
-- Name: class class_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class ALTER COLUMN class_id SET DEFAULT nextval('public.class_class_id_seq'::regclass);


--
-- TOC entry 3353 (class 2604 OID 66004)
-- Name: class_info class_info_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_info ALTER COLUMN class_info_id SET DEFAULT nextval('public.class_info_class_info_id_seq'::regclass);


--
-- TOC entry 3362 (class 2604 OID 74168)
-- Name: class_payments class_payment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_payments ALTER COLUMN class_payment_id SET DEFAULT nextval('public.class_payments_class_payment_id_seq'::regclass);


--
-- TOC entry 3351 (class 2604 OID 65983)
-- Name: class_reviews review_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_reviews ALTER COLUMN review_id SET DEFAULT nextval('public.reviews_review_id_seq'::regclass);


--
-- TOC entry 3334 (class 2604 OID 65764)
-- Name: classtype class_type_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classtype ALTER COLUMN class_type_id SET DEFAULT nextval('public.classtype_class_type_id_seq'::regclass);


--
-- TOC entry 3335 (class 2604 OID 65765)
-- Name: classvisits visit_class_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classvisits ALTER COLUMN visit_class_id SET DEFAULT nextval('public.classvisits_visit_class_id_seq'::regclass);


--
-- TOC entry 3337 (class 2604 OID 65766)
-- Name: client client_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client ALTER COLUMN client_id SET DEFAULT nextval('public.client_client_id_seq'::regclass);


--
-- TOC entry 3368 (class 2604 OID 82471)
-- Name: client_transaction transaction_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client_transaction ALTER COLUMN transaction_id SET DEFAULT nextval('public.client_transaction_transaction_id_seq'::regclass);


--
-- TOC entry 3340 (class 2604 OID 65767)
-- Name: clientmembership client_membership_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clientmembership ALTER COLUMN client_membership_id SET DEFAULT nextval('public.clientmembership_client_membership_id_seq'::regclass);


--
-- TOC entry 3342 (class 2604 OID 65768)
-- Name: equipment equipment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipment ALTER COLUMN equipment_id SET DEFAULT nextval('public.equipment_equipment_id_seq'::regclass);


--
-- TOC entry 3343 (class 2604 OID 65769)
-- Name: equipmentcondition equipment_condition_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipmentcondition ALTER COLUMN equipment_condition_id SET DEFAULT nextval('public.equipmentcondition_equipment_condition_id_seq'::regclass);


--
-- TOC entry 3344 (class 2604 OID 65770)
-- Name: hall hall_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hall ALTER COLUMN hall_id SET DEFAULT nextval('public.hall_hall_id_seq'::regclass);


--
-- TOC entry 3356 (class 2604 OID 66063)
-- Name: lockers locker_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.lockers ALTER COLUMN locker_id SET DEFAULT nextval('public.lockers_locker_id_seq'::regclass);


--
-- TOC entry 3345 (class 2604 OID 65771)
-- Name: membership membership_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership ALTER COLUMN membership_id SET DEFAULT nextval('public.membership_membership_id_seq'::regclass);


--
-- TOC entry 3364 (class 2604 OID 74202)
-- Name: membership_payments membership_payment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_payments ALTER COLUMN membership_payment_id SET DEFAULT nextval('public.membership_payments_membership_payment_id_seq'::regclass);


--
-- TOC entry 3361 (class 2604 OID 66110)
-- Name: membership_services membershipservice_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_services ALTER COLUMN membershipservice_id SET DEFAULT nextval('public.membership_services_membershipservice_id_seq'::regclass);


--
-- TOC entry 3346 (class 2604 OID 65773)
-- Name: membershiptype membership_type_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membershiptype ALTER COLUMN membership_type_id SET DEFAULT nextval('public.membershiptype_membership_type_id_seq'::regclass);


--
-- TOC entry 3357 (class 2604 OID 66070)
-- Name: rented_lockers rent_locker_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rented_lockers ALTER COLUMN rent_locker_id SET DEFAULT nextval('public.rented_lockers_rent_locker_id_seq'::regclass);


--
-- TOC entry 3347 (class 2604 OID 65774)
-- Name: roles role_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles ALTER COLUMN role_id SET DEFAULT nextval('public.roles_role_id_seq'::regclass);


--
-- TOC entry 3358 (class 2604 OID 66088)
-- Name: service_type service_type_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.service_type ALTER COLUMN service_type_id SET DEFAULT nextval('public.service_type_service_type_id_seq'::regclass);


--
-- TOC entry 3359 (class 2604 OID 66095)
-- Name: services service_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services ALTER COLUMN service_id SET DEFAULT nextval('public.services_service_id_seq'::regclass);


--
-- TOC entry 3363 (class 2604 OID 74185)
-- Name: services_payments service_payment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services_payments ALTER COLUMN service_payment_id SET DEFAULT nextval('public.services_payments_service_payment_id_seq'::regclass);


--
-- TOC entry 3348 (class 2604 OID 65776)
-- Name: specialization specialization_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.specialization ALTER COLUMN specialization_id SET DEFAULT nextval('public.specialization_specialization_id_seq'::regclass);


--
-- TOC entry 3365 (class 2604 OID 82393)
-- Name: trainer trainer_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer ALTER COLUMN trainer_id SET DEFAULT nextval('public.trainer_trainer_id_seq'::regclass);


--
-- TOC entry 3354 (class 2604 OID 66018)
-- Name: trainer_reviews trainer_review_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer_reviews ALTER COLUMN trainer_review_id SET DEFAULT nextval('public.trainer_reviews_trainer_review_id_seq'::regclass);


--
-- TOC entry 3366 (class 2604 OID 82448)
-- Name: training_plan training_plan_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.training_plan ALTER COLUMN training_plan_id SET DEFAULT nextval('public.training_plan_training_plan_id_seq'::regclass);


--
-- TOC entry 3349 (class 2604 OID 65778)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 3350 (class 2604 OID 65779)
-- Name: workschedule work_schedule_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.workschedule ALTER COLUMN work_schedule_id SET DEFAULT nextval('public.workschedule_work_schedule_id_seq'::regclass);


--
-- TOC entry 3415 (class 2606 OID 66008)
-- Name: class_info class_info_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_info
    ADD CONSTRAINT class_info_pkey PRIMARY KEY (class_info_id);


--
-- TOC entry 3429 (class 2606 OID 74170)
-- Name: class_payments class_payments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_payments
    ADD CONSTRAINT class_payments_pkey PRIMARY KEY (class_payment_id);


--
-- TOC entry 3375 (class 2606 OID 65781)
-- Name: class class_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT class_pkey PRIMARY KEY (class_id);


--
-- TOC entry 3377 (class 2606 OID 65783)
-- Name: classtype classtype_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classtype
    ADD CONSTRAINT classtype_pkey PRIMARY KEY (class_type_id);


--
-- TOC entry 3379 (class 2606 OID 65785)
-- Name: classvisits classvisits_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classvisits
    ADD CONSTRAINT classvisits_pkey PRIMARY KEY (visit_class_id);


--
-- TOC entry 3381 (class 2606 OID 65787)
-- Name: client client_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_pkey PRIMARY KEY (client_id);


--
-- TOC entry 3441 (class 2606 OID 82476)
-- Name: client_transaction client_transaction_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client_transaction
    ADD CONSTRAINT client_transaction_pkey PRIMARY KEY (transaction_id);


--
-- TOC entry 3385 (class 2606 OID 65789)
-- Name: clientmembership clientmembership_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clientmembership
    ADD CONSTRAINT clientmembership_pkey PRIMARY KEY (client_membership_id);


--
-- TOC entry 3387 (class 2606 OID 65791)
-- Name: equipment equipment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipment
    ADD CONSTRAINT equipment_pkey PRIMARY KEY (equipment_id);


--
-- TOC entry 3389 (class 2606 OID 65793)
-- Name: equipmentcondition equipmentcondition_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipmentcondition
    ADD CONSTRAINT equipmentcondition_pkey PRIMARY KEY (equipment_condition_id);


--
-- TOC entry 3391 (class 2606 OID 65795)
-- Name: gender gender_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.gender
    ADD CONSTRAINT gender_pkey PRIMARY KEY (gender_id);


--
-- TOC entry 3393 (class 2606 OID 65797)
-- Name: hall hall_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hall
    ADD CONSTRAINT hall_pkey PRIMARY KEY (hall_id);


--
-- TOC entry 3395 (class 2606 OID 65799)
-- Name: hallequipment hallequipment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hallequipment
    ADD CONSTRAINT hallequipment_pkey PRIMARY KEY (equipment_id, hall_id);


--
-- TOC entry 3419 (class 2606 OID 66065)
-- Name: lockers lockers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.lockers
    ADD CONSTRAINT lockers_pkey PRIMARY KEY (locker_id);


--
-- TOC entry 3433 (class 2606 OID 74204)
-- Name: membership_payments membership_payments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_payments
    ADD CONSTRAINT membership_payments_pkey PRIMARY KEY (membership_payment_id);


--
-- TOC entry 3397 (class 2606 OID 65801)
-- Name: membership membership_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership
    ADD CONSTRAINT membership_pkey PRIMARY KEY (membership_id);


--
-- TOC entry 3427 (class 2606 OID 66112)
-- Name: membership_services membership_services_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_services
    ADD CONSTRAINT membership_services_pkey PRIMARY KEY (membershipservice_id);


--
-- TOC entry 3399 (class 2606 OID 65805)
-- Name: membershiptype membershiptype_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membershiptype
    ADD CONSTRAINT membershiptype_pkey PRIMARY KEY (membership_type_id);


--
-- TOC entry 3421 (class 2606 OID 66072)
-- Name: rented_lockers rented_lockers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rented_lockers
    ADD CONSTRAINT rented_lockers_pkey PRIMARY KEY (rent_locker_id);


--
-- TOC entry 3413 (class 2606 OID 65989)
-- Name: class_reviews reviews_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_reviews
    ADD CONSTRAINT reviews_pkey PRIMARY KEY (review_id);


--
-- TOC entry 3401 (class 2606 OID 65807)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 3403 (class 2606 OID 65809)
-- Name: roles roles_role_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_role_name_key UNIQUE (role_name);


--
-- TOC entry 3423 (class 2606 OID 66090)
-- Name: service_type service_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.service_type
    ADD CONSTRAINT service_type_pkey PRIMARY KEY (service_type_id);


--
-- TOC entry 3431 (class 2606 OID 74187)
-- Name: services_payments services_payments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services_payments
    ADD CONSTRAINT services_payments_pkey PRIMARY KEY (service_payment_id);


--
-- TOC entry 3425 (class 2606 OID 66100)
-- Name: services services_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services
    ADD CONSTRAINT services_pkey PRIMARY KEY (service_id);


--
-- TOC entry 3405 (class 2606 OID 65813)
-- Name: specialization specialization_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.specialization
    ADD CONSTRAINT specialization_pkey PRIMARY KEY (specialization_id);


--
-- TOC entry 3435 (class 2606 OID 82398)
-- Name: trainer trainer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT trainer_pkey PRIMARY KEY (trainer_id);


--
-- TOC entry 3417 (class 2606 OID 66024)
-- Name: trainer_reviews trainer_reviews_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer_reviews
    ADD CONSTRAINT trainer_reviews_pkey PRIMARY KEY (trainer_review_id);


--
-- TOC entry 3439 (class 2606 OID 82453)
-- Name: training_plan training_plan_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.training_plan
    ADD CONSTRAINT training_plan_pkey PRIMARY KEY (training_plan_id);


--
-- TOC entry 3383 (class 2606 OID 65930)
-- Name: client uk_client_passport; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT uk_client_passport UNIQUE (passport_series, passport_number);


--
-- TOC entry 3437 (class 2606 OID 82433)
-- Name: trainer uk_trainer_passport; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT uk_trainer_passport UNIQUE (passport_series, passport_number);


--
-- TOC entry 3407 (class 2606 OID 65819)
-- Name: users users_login_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_login_key UNIQUE (login);


--
-- TOC entry 3409 (class 2606 OID 65821)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 3411 (class 2606 OID 65823)
-- Name: workschedule workschedule_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.workschedule
    ADD CONSTRAINT workschedule_pkey PRIMARY KEY (work_schedule_id);


--
-- TOC entry 3477 (class 2620 OID 65950)
-- Name: client trg_client_delete_user; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_client_delete_user AFTER DELETE ON public.client FOR EACH ROW EXECUTE FUNCTION public.delete_user_when_client_deleted();


--
-- TOC entry 3481 (class 2620 OID 82486)
-- Name: class_reviews trg_empty_review_class; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_empty_review_class BEFORE INSERT ON public.class_reviews FOR EACH ROW EXECUTE FUNCTION public.empty_review_check();


--
-- TOC entry 3482 (class 2620 OID 82487)
-- Name: trainer_reviews trg_empty_review_trainer; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_empty_review_trainer BEFORE INSERT ON public.trainer_reviews FOR EACH ROW EXECUTE FUNCTION public.empty_review_check();


--
-- TOC entry 3478 (class 2620 OID 65957)
-- Name: client trg_insert_user; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_insert_user BEFORE INSERT ON public.client FOR EACH ROW EXECUTE FUNCTION public.insert_user_on_client();


--
-- TOC entry 3480 (class 2620 OID 82490)
-- Name: hallequipment trg_limit_hall_equipment; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_limit_hall_equipment BEFORE INSERT ON public.hallequipment FOR EACH ROW EXECUTE FUNCTION public.limit_equipment_quantity();


--
-- TOC entry 3479 (class 2620 OID 82484)
-- Name: equipment trg_set_maintenance_date; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_set_maintenance_date BEFORE INSERT ON public.equipment FOR EACH ROW EXECUTE FUNCTION public.set_maintenance_date();


--
-- TOC entry 3442 (class 2606 OID 65824)
-- Name: class class_class_type_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT class_class_type_id_fkey FOREIGN KEY (class_type_id) REFERENCES public.classtype(class_type_id);


--
-- TOC entry 3443 (class 2606 OID 65829)
-- Name: class class_hall_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT class_hall_id_fkey FOREIGN KEY (hall_id) REFERENCES public.hall(hall_id);


--
-- TOC entry 3465 (class 2606 OID 74171)
-- Name: class_payments class_payments_class_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_payments
    ADD CONSTRAINT class_payments_class_id_fkey FOREIGN KEY (class_id) REFERENCES public.class(class_id);


--
-- TOC entry 3466 (class 2606 OID 74176)
-- Name: class_payments class_payments_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_payments
    ADD CONSTRAINT class_payments_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.client(client_id);


--
-- TOC entry 3457 (class 2606 OID 66035)
-- Name: class_reviews class_reviews_author_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_reviews
    ADD CONSTRAINT class_reviews_author_id_fkey FOREIGN KEY (author_id) REFERENCES public.client(client_id) ON DELETE SET NULL;


--
-- TOC entry 3444 (class 2606 OID 65834)
-- Name: class class_work_schedule_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT class_work_schedule_id_fkey FOREIGN KEY (work_schedule_id) REFERENCES public.workschedule(work_schedule_id);


--
-- TOC entry 3446 (class 2606 OID 65839)
-- Name: classvisits classvisits_class_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classvisits
    ADD CONSTRAINT classvisits_class_id_fkey FOREIGN KEY (class_id) REFERENCES public.class(class_id);


--
-- TOC entry 3447 (class 2606 OID 65844)
-- Name: classvisits classvisits_client_membership_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classvisits
    ADD CONSTRAINT classvisits_client_membership_id_fkey FOREIGN KEY (client_membership_id) REFERENCES public.clientmembership(client_membership_id);


--
-- TOC entry 3448 (class 2606 OID 65849)
-- Name: client client_gender_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_gender_id_fkey FOREIGN KEY (gender_id) REFERENCES public.gender(gender_id);


--
-- TOC entry 3476 (class 2606 OID 82477)
-- Name: client_transaction client_transaction_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client_transaction
    ADD CONSTRAINT client_transaction_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.client(client_id);


--
-- TOC entry 3449 (class 2606 OID 65935)
-- Name: client client_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id) ON DELETE CASCADE;


--
-- TOC entry 3450 (class 2606 OID 65859)
-- Name: clientmembership clientmembership_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clientmembership
    ADD CONSTRAINT clientmembership_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.client(client_id);


--
-- TOC entry 3451 (class 2606 OID 65864)
-- Name: clientmembership clientmembership_membership_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clientmembership
    ADD CONSTRAINT clientmembership_membership_id_fkey FOREIGN KEY (membership_id) REFERENCES public.membership(membership_id);


--
-- TOC entry 3452 (class 2606 OID 65869)
-- Name: equipment equipment_equipment_condition_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipment
    ADD CONSTRAINT equipment_equipment_condition_id_fkey FOREIGN KEY (equipment_condition_id) REFERENCES public.equipmentcondition(equipment_condition_id);


--
-- TOC entry 3445 (class 2606 OID 66009)
-- Name: class fk_class_info; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT fk_class_info FOREIGN KEY (class_info_id) REFERENCES public.class_info(class_info_id);


--
-- TOC entry 3453 (class 2606 OID 65874)
-- Name: hallequipment hallequipment_equipment_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hallequipment
    ADD CONSTRAINT hallequipment_equipment_id_fkey FOREIGN KEY (equipment_id) REFERENCES public.equipment(equipment_id);


--
-- TOC entry 3454 (class 2606 OID 65879)
-- Name: hallequipment hallequipment_hall_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hallequipment
    ADD CONSTRAINT hallequipment_hall_id_fkey FOREIGN KEY (hall_id) REFERENCES public.hall(hall_id);


--
-- TOC entry 3455 (class 2606 OID 65884)
-- Name: membership membership_membership_type_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership
    ADD CONSTRAINT membership_membership_type_id_fkey FOREIGN KEY (membership_type_id) REFERENCES public.membershiptype(membership_type_id);


--
-- TOC entry 3469 (class 2606 OID 74210)
-- Name: membership_payments membership_payments_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_payments
    ADD CONSTRAINT membership_payments_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.client(client_id);


--
-- TOC entry 3470 (class 2606 OID 74205)
-- Name: membership_payments membership_payments_membership_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_payments
    ADD CONSTRAINT membership_payments_membership_id_fkey FOREIGN KEY (membership_id) REFERENCES public.membership(membership_id);


--
-- TOC entry 3463 (class 2606 OID 66113)
-- Name: membership_services membership_services_client_membership_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_services
    ADD CONSTRAINT membership_services_client_membership_id_fkey FOREIGN KEY (client_membership_id) REFERENCES public.clientmembership(client_membership_id) ON DELETE CASCADE;


--
-- TOC entry 3464 (class 2606 OID 66118)
-- Name: membership_services membership_services_service_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.membership_services
    ADD CONSTRAINT membership_services_service_id_fkey FOREIGN KEY (service_id) REFERENCES public.services(service_id) ON DELETE CASCADE;


--
-- TOC entry 3460 (class 2606 OID 66078)
-- Name: rented_lockers rented_lockers_client_membership_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rented_lockers
    ADD CONSTRAINT rented_lockers_client_membership_id_fkey FOREIGN KEY (client_membership_id) REFERENCES public.clientmembership(client_membership_id) ON DELETE CASCADE;


--
-- TOC entry 3461 (class 2606 OID 66073)
-- Name: rented_lockers rented_lockers_locker_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rented_lockers
    ADD CONSTRAINT rented_lockers_locker_id_fkey FOREIGN KEY (locker_id) REFERENCES public.lockers(locker_id) ON DELETE CASCADE;


--
-- TOC entry 3458 (class 2606 OID 66040)
-- Name: class_reviews reviews_class_info_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class_reviews
    ADD CONSTRAINT reviews_class_info_id_fkey FOREIGN KEY (class_info_id) REFERENCES public.class_info(class_info_id) ON DELETE SET NULL;


--
-- TOC entry 3467 (class 2606 OID 74193)
-- Name: services_payments services_payments_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services_payments
    ADD CONSTRAINT services_payments_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.client(client_id);


--
-- TOC entry 3468 (class 2606 OID 74188)
-- Name: services_payments services_payments_service_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services_payments
    ADD CONSTRAINT services_payments_service_id_fkey FOREIGN KEY (service_id) REFERENCES public.services(service_id);


--
-- TOC entry 3462 (class 2606 OID 66101)
-- Name: services services_service_type_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services
    ADD CONSTRAINT services_service_type_id_fkey FOREIGN KEY (service_type_id) REFERENCES public.service_type(service_type_id) ON DELETE CASCADE;


--
-- TOC entry 3471 (class 2606 OID 82401)
-- Name: trainer trainer_gender_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT trainer_gender_id_fkey FOREIGN KEY (gender_id) REFERENCES public.gender(gender_id);


--
-- TOC entry 3459 (class 2606 OID 66030)
-- Name: trainer_reviews trainer_reviews_author_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer_reviews
    ADD CONSTRAINT trainer_reviews_author_id_fkey FOREIGN KEY (author_id) REFERENCES public.client(client_id) ON DELETE SET NULL;


--
-- TOC entry 3472 (class 2606 OID 82406)
-- Name: trainer trainer_specialization_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT trainer_specialization_id_fkey FOREIGN KEY (specialization_id) REFERENCES public.specialization(specialization_id);


--
-- TOC entry 3473 (class 2606 OID 82411)
-- Name: trainer trainer_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT trainer_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id) ON DELETE CASCADE;


--
-- TOC entry 3474 (class 2606 OID 82454)
-- Name: training_plan training_plan_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.training_plan
    ADD CONSTRAINT training_plan_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.client(client_id) ON DELETE CASCADE;


--
-- TOC entry 3475 (class 2606 OID 82459)
-- Name: training_plan training_plan_trainer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.training_plan
    ADD CONSTRAINT training_plan_trainer_id_fkey FOREIGN KEY (trainer_id) REFERENCES public.trainer(trainer_id) ON DELETE CASCADE;


--
-- TOC entry 3456 (class 2606 OID 65914)
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.roles(role_id) ON DELETE CASCADE;


-- Completed on 2025-05-05 01:39:41

--
-- PostgreSQL database dump complete
--

