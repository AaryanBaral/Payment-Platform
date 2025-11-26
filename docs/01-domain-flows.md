# 01 - Domain Flows  
**Core Functional Behaviors of the Payment Platform**

This document outlines the fundamental domain flows of the system.  
These flows describe *what the system must do* from a business perspective, independent of implementation details.

---

# 1. Overview

The platform supports multiple **tenants (businesses)** who manage **merchants**.  
Tenants record payments, the system computes revenue splits, maintains an immutable ledger,  
and manages payouts to merchants.

This document covers all primary flows:
- Tenant lifecycle  
- Merchant lifecycle  
- Payment lifecycle  
- Ledger flow  
- Payout lifecycle  
- Reporting flow  

---

# 2. Tenant Lifecycle Flow

## 2.1 Tenant Creation
1. Platform admin creates a new tenant.
2. System initializes:
   - Tenant record
   - Default tenant settings (timezone, payout schedule)
   - Initial tenant admin user

## 2.2 Tenant User Management
1. Tenant admin invites users (finance, viewer, etc.)
2. Users create password and join tenant.
3. Roles determine access:
   - **Admin** → manage merchants, payouts, users
   - **Finance** → manage payouts, view ledger
   - **Viewer** → read-only

## 2.3 Tenant Isolation
- Each tenant can only see its own data.
- Cross-tenant access is strictly forbidden.

---

# 3. Merchant Lifecycle Flow

## 3.1 Merchant Creation
1. Tenant admin creates a merchant with:
   - Merchant profile
   - Revenue share percentage (e.g., 80/20)
2. System validates:
   - Merchant belongs to the current tenant
   - Revenue share is valid

## 3.2 Merchant Management
- Tenant admin can update:
  - Merchant name, email, bank details
  - Revenue share
- Merchant cannot be deleted if payments exist (soft disable only).

---

# 4. Payment Lifecycle Flow

## 4.1 Payment Initiated (Tenant → Platform)
1. Customer pays on tenant’s website using tenant’s payment gateway.
2. Tenant backend notifies our system:
   - Payment amount
   - Merchant ID
   - External payment reference
3. System stores Payment with **Pending** status.

## 4.2 Payment Gateway Callback
1. Gateway (Khalti/eSewa/etc.) sends a secure callback:
   - Success or Failure
   - External payment ID
   - Signature
2. System validates authenticity.

## 4.3 On Payment Success
1. Update payment → **Succeeded**
2. Calculate:
   - Merchant share
   - Platform share
3. Create **ledger entries**:
   - Merchant credit
   - Platform credit
4. Increase merchant Available Balance.

## 4.4 On Payment Failure
- Update payment → **Failed**
- No ledger entry created.

---

# 5. Ledger Flow

## 5.1 Ledger Characteristics
- Immutable
- Append-only
- Accurate history for all money movements

## 5.2 Ledger Entry Triggers
- Payment succeeded → credit merchant + platform
- Payout completed → debit merchant
- Adjustments (admin)

## 5.3 Ledger Uses
- Reporting
- Merchant balances
- Reconciliation
- Audits

---

# 6. Payout Lifecycle Flow

## 6.1 Merchant Earns Funds
- Merchant accumulates balance via successful payments.

## 6.2 Create Payout Request
Two paths:
- Manual: Finance team triggers payout
- Automatic: Based on tenant schedule

Flow:
1. Check merchant balance.
2. Create “Payout Requested”.

## 6.3 Approval
1. Finance/admin reviews.
2. Approves or rejects payout.

## 6.4 Actual Transfer
- Tenant sends money via bank/eSewa/Khalti/etc.
- System does NOT transfer money.

## 6.5 Confirmation
1. Tenant confirms payout as paid.
2. System creates **ledger debit**.
3. Payout → **Completed**

---

# 7. Reporting Flow

The platform provides:
- Merchant earning reports
- Tenant revenue summaries
- Payment logs
- Payout history
- Ledger export

Reports include:
- Pagination
- Date ranges
- Filters

---

# End of File  