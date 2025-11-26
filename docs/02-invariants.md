# 02 - System Invariants  
**Rules That Must Always Hold True in the Platform**

An invariant is a rule that must *always* remain true to keep the system consistent.  
If an invariant is violated, the system is in an invalid state.

These invariants guide:
- Domain model design
- Application logic
- Database constraints
- Tests
- Architectural decisions (e.g., multi-tenancy)

---

# 1. Tenant & User Invariants

### [T1] All tenant-owned data must include a TenantId.
Applies to: Merchant, Payment, Payout, LedgerEntry, TenantUser, etc.

### [T2] Tenant A must never access Tenant B's data.
Enforced by global query filters and application services.

### [T3] Each merchant belongs to exactly one tenant.

### [T4] Tenant roles apply only within that tenant.
A user’s access in Tenant A does not apply to Tenant B.

---

# 2. Merchant Invariants

### [M1] Merchant revenue share must be within 0–100% (exclusive).
Invalid shares must be rejected.

### [M2] Merchants cannot be hard deleted if transactions exist.
Use disable/archival instead.

### [M3] Merchant Available Balance must never drop below zero.

---

# 3. Payment Invariants

### [P1] Valid payment transitions:
- Pending → Succeeded  
- Pending → Failed  

### [P2] Once a payment is Succeeded or Failed, it cannot change state.

### [P3] Every payment belongs to exactly:
- one merchant  
- one tenant  

### [P4] Only verified payment gateway callbacks may mark a payment as succeeded.

### [P5] Payment amounts must always be positive.

---

# 4. Ledger Invariants

### [L1] Ledger entries must be immutable after creation.
No updates or deletes.

### [L2] All financial events must produce ledger entries.
Examples:
- Payment succeeded
- Payout completed
- Manual adjustments

### [L3] Ledger balance formula must always hold:
MerchantBalance = Sum(Credits) - Sum(Debits)


### [L4] All ledger timestamps must be stored in UTC.

---

# 5. Payout Invariants

### [O1] Payout amount cannot exceed merchant’s available balance at creation time.

### [O2] Valid payout transitions:
- Requested → Approved → Paid  
or  
- Requested → Rejected  

### [O3] When payout becomes **Paid**, a corresponding ledger debit must be created.

### [O4] Payout cannot be marked Paid more than once.

---

# 6. Reporting & Query Invariants

### [R1] All list endpoints require pagination.
No unbounded queries allowed.

### [R2] Read queries must never mutate domain state.

### [R3] Reports must always respect tenant isolation.

---

# 7. System & Security Invariants

### [S1] Every write operation must identify the tenant.
Using JWT claim or subdomain header.

### [S2] Sensitive actions (payout approval) require privileged roles.

### [S3] All external callbacks must be signature-verified.

### [S4] Secrets (JWT keys, DB credentials, API keys) must not be stored in source control.

---

# End of File