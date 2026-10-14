// Features/EmailAccounts, EmailSenders and WhatsAppAccounts contract.
// There is no GET by id: records are edited from the list data.

/** GET /email-accounts item (EmailAccountSummary: the app password is never returned). */
export type EmailAccount = {
  companyId: number;
  id: number;
  emailAddress?: string;
  imapHost?: string;
  imapPort: number;
  isActive: boolean;
  lastCheckedAt?: string;
};

/** GET /email-senders item */
export type EmailSender = {
  companyId: number;
  id: number;
  senderAddress?: string;
  description?: string;
  isActive: boolean;
};

/** GET /whatsapp-accounts item (WhatsAppAccountSummary: the access token is never returned). */
export type WhatsAppAccount = {
  companyId: number;
  id: number;
  phoneNumberId?: string;
  isActive: boolean;
};
