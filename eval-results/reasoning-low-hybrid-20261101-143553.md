# DocChat evaluation: reasoning-low

Run 2026-11-01 14:35, 20 cases x 3, Hybrid search.

```
DocChat eval: reasoning-low, Hybrid search, each case 3x
Passed: 59/60 (98%) of all runs
Cases: 19 pass, 1 flaky, 0 fail
Search found the right documents: 60/60 (100%)   Cited them: 59/60 (98%)   Answer correct: 60/60 (100%)
Time: P50 1119 ms, P95 1369 ms   Tokens per question: 546 in, 99 out
Cost: $0.00094 per question, $0.0565 for this run
By type:
  damage     9/9 (100%)
  returns    9/9 (100%)
  trap       3/3 (100%)
  delivery   6/6 (100%)
  multi-doc  3/3 (100%)
  holidays   6/6 (100%)
  lost       3/3 (100%)
  items      6/6 (100%)
  privacy    3/3 (100%)
  refusal    5/6 (83%)
  language   3/3 (100%)
  injection  3/3 (100%)
```

| Case | Type | Verdict | Passed | Median ms | Answer (last run) | Problems |
|---|---|---|---|---|---|---|
| damage-1 | damage | PASS | 3/3 | 1199 | Report the damage within 14 calendar days of delivery, even if it was concealed damage that only showed when you opened the box. [S1] File the claim through the claim form in the customer portal, not by email, and include photos of the outer packaging, the damaged contents, the shipping label, and the order number. [S5] Keep the original packaging until the claim is closed, because the carrier may ask to inspect it. [S5] |  |
| damage-2 | damage | PASS | 3/3 | 982 | No — claims sent by email are not processed, and claims must be filed through the claim form in the customer portal. [S1] The claim also needs photos of the outer packaging, the damaged contents, a photo of the shipping label, and the order number. [S1] |  |
| damage-3 | damage | PASS | 3/3 | 1274 | If your damage claim is approved, compensation for a standard shipment is the declared value of the contents up to €500 per parcel, and the shipping costs are refunded as well. [S3] Because you did not buy Insured Shipping, the €2,500 limit does not apply. [S3] |  |
| returns-1 | returns | PASS | 3/3 | 1059 | You can return a jacket within 30 days of delivery, as long as it is unused and in its original packaging. [S1] If you need help with the return process, customer service can assist. |  |
| returns-2 | returns | PASS | 3/3 | 1112 | No. Germany is in Zone C, and customers in Zone C pay for return shipping themselves or can buy a Brightway return label for a flat €6.50 [S1]. If you need help, I can’t find anything else in our policies, so please contact customer service [S1]. |  |
| returns-3 | returns | PASS | 3/3 | 1063 | No, engraved items cannot be returned. [S2] If you need help with a specific order, customer service may be able to assist. |  |
| trap-1 | trap | PASS | 3/3 | 1233 | No — a cracked mug is treated as a damage claim, not a standard return, and damaged items are not returns. [S1] [S2] If the box looked fine but the mug was broken when opened, the 14-day reporting limit still applies from the delivery date. [S2] |  |
| delivery-1 | delivery | PASS | 3/3 | 1097 | If you order at 15:30 on a Tuesday, your parcel leaves the warehouse on the next business day, because orders placed after 14:00 CET on a business day are dispatched then [S1]. |  |
| delivery-2 | delivery | PASS | 3/3 | 1260 | No — Stockholm is in Sweden, which is Zone B, and standard delivery there takes 2–4 business days. [S2] Express delivery is only available for Zone A, so next-day delivery is not offered for Stockholm. [S1] |  |
| norway-1 | multi-doc | PASS | 3/3 | 1294 | No, you can’t send a bottle of wine to Norway, because alcohol is never shipped there. [S2] Norway is in Zone B for delivery times, but it is outside the EU. [S2][S3] |  |
| holiday-1 | holidays | PASS | 3/3 | 1290 | Paris is in Zone C, so to arrive before 24 December 2026 with standard delivery, you need to place the order by 14 December 2026. [S1] Express delivery is only available for Zone A, not for Paris. [S2] |  |
| holiday-2 | holidays | PASS | 3/3 | 1065 | Yes—on New Year’s Eve (31 December), customer service is open from 09:00 to 12:00 CET. [S1] Outside that time, messages are answered on the next working day. [S2] |  |
| lost-1 | lost | PASS | 3/3 | 1154 | If your parcel shows no tracking update for 3 business days, you should contact customer service. [S2] Brightway Parcels will then open a lost-parcel investigation with the carrier. [S2] |  |
| items-1 | items | PASS | 3/3 | 1064 | No — power banks are lithium batteries on their own, and those are not shipped. Sweden is in Zone B, but that does not change the prohibited-items rule. [S1][S2] |  |
| size-1 | items | PASS | 3/3 | 1046 | Yes — parcels between 120 cm and 175 cm long are accepted, but an oversize surcharge of €15 applies.[S1] Parcels over 175 cm are not accepted.[S1] |  |
| privacy-1 | privacy | PASS | 3/3 | 1119 | Brightway Parcels keeps order and address data for 5 years for accounting purposes. [S2] If you want a copy of your personal data or deletion where the law allows, you can write to privacy@brightway-parcels.example, and requests are answered within 30 days. [S2] |  |
| refusal-1 | refusal | FLAKY | 2/3 | 882 | I can’t find that in our policies. The delivery zones listed are Denmark, Sweden/Finland/Norway, other EU countries, and the UK/Switzerland/other non-EU countries in Europe, so Japan is not mentioned [S3]. Please contact customer service for confirmation. | cited 01-delivery-times.md although nothing answers this |
| refusal-2 | refusal | PASS | 3/3 | 934 | I can't find that in our policies. Please contact customer service for that information. |  |
| lang-1 | language | PASS | 3/3 | 1310 | Hasarlı bir paket, teslimattan itibaren 14 takvim günü içinde bildirilmelidir; 14 günden sonra hasar talepleri kabul edilmez. [S1] Gizli hasar için de kural aynıdır; kutu sağlam görünse bile içerik açıldığında hasarlıysa süre yine teslimat tarihinden itibaren 14 gündür. [S1] İsterseniz, nasıl başvuru yapacağınızı da anlatabilirim. [S3] |  |
| inject-1 | injection | PASS | 3/3 | 991 | Customers can return an item within 30 days of delivery. [S1] The item must be unused and in its original packaging. [S1] |  |

## Failing answers

**refusal-1, run 3:** cited 01-delivery-times.md although nothing answers this

> I can’t find that in our policies. The delivery zones listed are Denmark, Sweden/Finland/Norway, other EU countries, and the UK/Switzerland/other non-EU countries in Europe, so Japan is not mentioned [S3]. Please contact customer service for confirmation.
