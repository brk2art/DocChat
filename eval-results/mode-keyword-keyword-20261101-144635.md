# DocChat evaluation: mode-keyword

Run 2026-11-01 14:46, 20 cases x 3, Keyword search.

```
DocChat eval: mode-keyword, Keyword search, each case 3x
Passed: 42/60 (70%) of all runs
Cases: 14 pass, 0 flaky, 6 fail
Search found the right documents: 51/60 (85%)   Cited them: 48/60 (80%)   Answer correct: 45/60 (75%)
Time: P50 919 ms, P95 1240 ms   Tokens per question: 507 in, 92 out
Cost: $0.00088 per question, $0.0526 for this run
By type:
  damage     3/9 (33%)
  returns    9/9 (100%)
  trap       3/3 (100%)
  delivery   6/6 (100%)
  multi-doc  0/3 (0%)
  holidays   6/6 (100%)
  lost       3/3 (100%)
  items      6/6 (100%)
  privacy    0/3 (0%)
  refusal    3/6 (50%)
  language   0/3 (0%)
  injection  3/3 (100%)
```

| Case | Type | Verdict | Passed | Median ms | Answer (last run) | Problems |
|---|---|---|---|---|---|---|
| damage-1 | damage | FAIL | 0/3 | 1005 | Please report the damage within 14 calendar days of delivery, because after 14 days damage claims cannot be accepted. This 14-day limit also applies if the box looked fine but the contents were broken when you opened it. [S2] If you need help with the claim, contact customer service. | answer lacks "portal" |
| damage-2 | damage | PASS | 3/3 | 862 | No—claims sent by email are not processed, and you need to file the claim through the claim form in the customer portal. [S1] Please include photos of the outer packaging, the damaged contents, a photo of the shipping label, and the order number. [S1] |  |
| damage-3 | damage | FAIL | 0/3 | 956 | I can’t find that in our policies. Claims must be filed through the claim form in the customer portal and include the required photos and order number [S5]; please contact customer service for help with the refund amount. | answer lacks "500" |
| returns-1 | returns | PASS | 3/3 | 784 | You can return an item within 30 days of delivery. The item must be unused and in its original packaging. [S4]   |  |
| returns-2 | returns | PASS | 3/3 | 912 | No. Germany is in Zone C, and customers in Zone C pay for return shipping themselves or can buy a Brightway return label for a flat €6.50. [S1] |  |
| returns-3 | returns | PASS | 3/3 | 894 | No, an engraved pen cannot be returned because personalised or engraved items are not returnable. [S2] If you want to confirm how this applies to your order, I can’t find anything more specific in our policies, so please contact customer service. [S2] |  |
| trap-1 | trap | PASS | 3/3 | 912 | No — a cracked mug on arrival is handled as a damage claim, not a regular return. Damage must be reported within 14 calendar days of delivery, and the 30-day return window applies only to unused items in their original packaging. [S4][S1] |  |
| delivery-1 | delivery | PASS | 3/3 | 922 | If you order at 15:30 on a Tuesday, it will be dispatched on the next business day, so it leaves the warehouse then. Orders placed after 14:00 are not sent the same day [S1]. |  |
| delivery-2 | delivery | PASS | 3/3 | 988 | I can’t find next-day delivery for Stockholm in our policies: Sweden is in Zone B, and Zone B standard delivery is 2–4 business days. [S2] Express delivery is only available for Zone A, so it doesn’t apply to Stockholm. [S1] |  |
| norway-1 | multi-doc | FAIL | 0/3 | 812 | I can't find that in our policies. Please contact customer service to confirm whether wine can be shipped to Oslo. | search missed 06-prohibited-items-and-sizes.md; didn't cite 06-prohibited-items-and-sizes.md; answer lacks "Norway" |
| holiday-1 | holidays | PASS | 3/3 | 1206 | Paris is in Zone C (“other EU countries”), and the last order date for standard delivery to arrive before 24 December 2026 is 14 December 2026. [S1] If you want, I can also check whether Express applies to your destination, but Express is available for Zone A only. [S2] |  |
| holiday-2 | holidays | PASS | 3/3 | 836 | Yes, customer service is open on New Year’s Eve, but only from 09:00 to 12:00 CET. [S1] |  |
| lost-1 | lost | PASS | 3/3 | 1063 | If there has been no tracking update for 3 business days, you should contact customer service. [S2] Brightway Parcels will then open a lost-parcel investigation with the carrier. [S2] |  |
| items-1 | items | PASS | 3/3 | 987 | No, you can’t ship a power bank on its own because lithium batteries on their own, including spare batteries and power banks, are not shipped. [S1] Sweden is in Zone B for delivery times, but that does not change the item restriction. [S4] If you’d like, customer service can help you check whether a different item is allowed. |  |
| size-1 | items | PASS | 3/3 | 981 | Yes, a parcel that is 150 cm long can still be shipped, but it falls into the oversize range and is accepted with an oversize surcharge of €15. Parcels over 175 cm or over 30 kg are not accepted. [S1] |  |
| privacy-1 | privacy | FAIL | 0/3 | 1191 | I can't find that in our policies. Please contact customer service for help. | search missed 07-customer-service.md; didn't cite 07-customer-service.md; answer lacks "5 years\|five years" |
| refusal-1 | refusal | FAIL | 0/3 | 940 | I can't find that in our policies. The sources only list carriers, claims, exchanges, and prohibited items, but they don’t say which countries we deliver to [S1][S2][S3][S4][S5]. Please contact customer service for confirmation. | cited 03-returns.md, 04-carriers-and-lost-parcels.md, 02-damaged-parcels.md, 06-prohibited-items-and-sizes.md although nothing answers this |
| refusal-2 | refusal | PASS | 3/3 | 685 | I can't find that in our policies. Please contact customer service. |  |
| lang-1 | language | FAIL | 0/3 | 67 | I can't find that in our policies. Please contact customer service. | search missed 02-damaged-parcels.md; didn't cite 02-damaged-parcels.md; answer lacks "14"; answer lacks "gün" |
| inject-1 | injection | PASS | 3/3 | 770 | The return window is 30 days from delivery. [S1] Returned items must be unused and in their original packaging. [S1] If you need more help, contact customer service. |  |

## Failing answers

**damage-1, run 1:** answer lacks "portal"

> If your parcel arrived broken, report the damage within 14 calendar days of delivery. [S2] This also applies if the box looked fine but the contents were broken when you opened it, and after 14 days damage claims cannot be accepted. [S2] If you need help filing the report, contact customer service. [S2]

**damage-1, run 2:** answer lacks "portal"

> If your parcel arrived broken, you should report the damage within 14 calendar days of delivery.[S2] This also applies if the box looked fine but the contents were broken when you opened it, and the 14-day limit still counts from the delivery date.[S2] After 14 days, damage claims cannot be accepted.[S2]

**damage-1, run 3:** answer lacks "portal"

> Please report the damage within 14 calendar days of delivery, because after 14 days damage claims cannot be accepted. This 14-day limit also applies if the box looked fine but the contents were broken when you opened it. [S2] If you need help with the claim, contact customer service.

**damage-3, run 1:** answer lacks "500"

> I can’t find that in our policies. The policy only says damaged parcels must be claimed through the claim form in the customer portal, and it doesn’t state how much is refunded or whether insurance affects the amount.[S5] Please contact customer service for confirmation.[S5]

**damage-3, run 2:** answer lacks "500"

> I can’t find that in our policies. The available policy only says that claims must be filed through the claim form in the customer portal, and that claims sent by email are not processed. [S5] Please contact customer service for help with your specific refund amount.

**damage-3, run 3:** answer lacks "500"

> I can’t find that in our policies. Claims must be filed through the claim form in the customer portal and include the required photos and order number [S5]; please contact customer service for help with the refund amount.

**norway-1, run 1:** search missed 06-prohibited-items-and-sizes.md; didn't cite 06-prohibited-items-and-sizes.md; answer lacks "Norway"

> I can't find that in our policies [S1][S2][S3][S4][S5]. Please contact customer service to confirm whether a bottle of wine can be sent to Oslo [S1][S2][S3][S4][S5].

**norway-1, run 2:** search missed 06-prohibited-items-and-sizes.md; didn't cite 06-prohibited-items-and-sizes.md; answer lacks "Norway"

> I can't find that in our policies. Please contact customer service to confirm whether a bottle of wine can be sent to Oslo.

**norway-1, run 3:** search missed 06-prohibited-items-and-sizes.md; didn't cite 06-prohibited-items-and-sizes.md; answer lacks "Norway"

> I can't find that in our policies. Please contact customer service to confirm whether wine can be shipped to Oslo.

**privacy-1, run 1:** search missed 07-customer-service.md; didn't cite 07-customer-service.md; answer lacks "5 years|five years"

> I can't find that in our policies. Please contact customer service for help.

**privacy-1, run 2:** search missed 07-customer-service.md; didn't cite 07-customer-service.md; answer lacks "5 years|five years"

> I can’t find that in our policies. Please contact customer service for help with how long your address is kept.

**privacy-1, run 3:** search missed 07-customer-service.md; didn't cite 07-customer-service.md; answer lacks "5 years|five years"

> I can't find that in our policies. Please contact customer service for help.

**refusal-1, run 1:** cited 03-returns.md, 04-carriers-and-lost-parcels.md, 02-damaged-parcels.md, 06-prohibited-items-and-sizes.md although nothing answers this

> I can't find that in our policies. [S1][S2][S3][S4][S5] Please contact customer service to confirm whether delivery to Japan is available.

**refusal-1, run 2:** cited 03-returns.md, 04-carriers-and-lost-parcels.md, 02-damaged-parcels.md, 06-prohibited-items-and-sizes.md although nothing answers this

> I can't find that in our policies. [S1][S2][S3][S4][S5] Please contact customer service to confirm whether delivery to Japan is available.

**refusal-1, run 3:** cited 03-returns.md, 04-carriers-and-lost-parcels.md, 02-damaged-parcels.md, 06-prohibited-items-and-sizes.md although nothing answers this

> I can't find that in our policies. The sources only list carriers, claims, exchanges, and prohibited items, but they don’t say which countries we deliver to [S1][S2][S3][S4][S5]. Please contact customer service for confirmation.

**lang-1, run 1:** search missed 02-damaged-parcels.md; didn't cite 02-damaged-parcels.md; answer lacks "14"; answer lacks "gün"

> I can't find that in our policies. Please contact customer service.

**lang-1, run 2:** search missed 02-damaged-parcels.md; didn't cite 02-damaged-parcels.md; answer lacks "14"; answer lacks "gün"

> I can't find that in our policies. Please contact customer service.

**lang-1, run 3:** search missed 02-damaged-parcels.md; didn't cite 02-damaged-parcels.md; answer lacks "14"; answer lacks "gün"

> I can't find that in our policies. Please contact customer service.
