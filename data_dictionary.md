# 🗄️ Data Dictionary & Schema Specification (LocalCRM)

## 📊 I. Core CRM Entities

### 1. Companies (`companies`)
| Field | Type | Constraint | Purpose |
| :--- | :--- | :--- | :--- |
| `company_id` | PK (Int) | Identity | Unique identifier. |
| `company_ref` | String(50) | Unique | Business reference number. |
| `name` | String(255) | Required | Legal name. |
| `city` | String(100) | | Headquarters location. |
| `company_type`| Enum | [Client, Partner, Vendor, Lead] | Category. |
| `rating` | Int | [1-5] | Quality score. |
| *(Audit Fields)* | See below | | Base entity behavior. |

### 2. Contacts (`contacts`)
| Field | Type | Constraint | Purpose |
| :--- | :--- | :--- | :--- |
| `contact_id` | PK (Int) | Identity | Unique identifier. |
| `first_name` | String(100) | Required | Personal name. |
| `last_name` | String(100) | Required | Family name. |
| `email` | String(255) | Unique | Contact address. |
| `rating` | Int | [1-5] | Priority score. |

### 3. Interactions (`interactions`)
| Field | Type | Constraint | Purpose |
| :--- | :--- | :--- | :--- |
| `interaction_id`| PK (Int) | Identity | Unique identifier. |
| `subject` | String(255) | Required | Brief summary. |
| `state` | Enum | [Planned, Completed, Cancelled] | Status. |
| `interaction_date`| Timestamp | Required | When it occurred. |
| `is_task` | Boolean | Default: FALSE | Indicates short-term task. |

---

## 🏗️ II. Support and Administrative Entities

### 1. Audit Logs (`audit_logs`)
| Field | Type | Purpose |
| :--- | :--- | :--- |
| `audit_id` | PK (Int) | Unique identifier. |
| `entity_name` | String | Type of entity affected. |
| `entity_id` | Int | ID of the specific record. |
| `action_type` | String | [CREATE, UPDATE, DELETE, RESTORE] |
| `performed_by`| String | Username of the actor. |
| `performed_at`| Timestamp | Execution time. |
| `notes` | String | **Change Summary** (e.g., 'City: London -> Paris'). |

### 2. Tags (`tags`) - First-Class Citizen
| Field | Type | Purpose |
| :--- | :--- | :--- |
| `tag_id` | PK (Int) | Unique identifier. |
| `tag_group` | String | Category (e.g., 'Industry', 'LeadSource'). |
| `tag_name` | String | Display name. |
| `tag_value` | String | Metadata value. |

---

## 🛡️ III. Global Entity Constraints (NFR-1)

All core business entities inherit from a standard base class containing:

| Field | Type | Behavior |
| :--- | :--- | :--- |
| `is_deleted` | Boolean | Soft-delete flag (default: FALSE). |
| `deleted_at` | Timestamp | Set during deletion (NULL otherwise). |
| `created_at` | Timestamp | Auto-populated on creation. |
| `updated_at` | Timestamp | **Optimistic Concurrency** validation token. |
| `created_by` | String | Actor who created the record. |
| `updated_by` | String | Actor who last modified the record. |
