# DocChat evaluation: mode-vector

Run 2026-11-01 14:48, 20 cases x 3, Vector search.

```
DocChat eval: mode-vector, Vector search, each case 3x
Passed: 58/60 (97%) of all runs
Cases: 19 pass, 1 flaky, 0 fail
Search found the right documents: 60/60 (100%)   Cited them: 58/60 (97%)   Answer correct: 60/60 (100%)
Time: P50 1125 ms, P95 1558 ms   Tokens per question: 548 in, 107 out
Cost: $0.00099 per question, $0.0592 for this run
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
  refusal    4/6 (67%)
  language   3/3 (100%)
  injection  3/3 (100%)
```

| Case | Type | Verdict | Passed | Median ms | Answer (last run) | Problems |
|---|---|---|---|---|---|---|
| damage-1 | damage | PASS | 3/3 | 1551 | Report the damage within 14 calendar days of delivery; after that, damage claims cannot be accepted, including concealed damage. [S2] Submit the claim through the claim form in the customer portal, not by email, and include photos of the outer packaging, the damaged contents, the shipping label, and the order number. [S1] Keep the original packaging until the claim is closed, because the carrier may ask to inspect it. [S1] |  |
| damage-2 | damage | PASS | 3/3 | 1078 | No. Claims sent by email are not processed; you need to file the claim through the claim form in the customer portal and include the required photos and order number. [S1] |  |
| damage-3 | damage | PASS | 3/3 | 1312 | For a damaged parcel without Insured Shipping, compensation covers the declared value of the contents up to €500 per parcel, and shipping costs are refunded as well if the claim is approved. [S1] So for your €1,200 laptop, you would get up to €500 back, plus the shipping cost if the claim is approved. [S1] |  |
| returns-1 | returns | PASS | 3/3 | 1046 | You can return a jacket within 30 days of delivery, as long as it is unused and in its original packaging. [S1] If you want, I can also tell you what happens with the refund after the return is received. [S2] |  |
| returns-2 | returns | PASS | 3/3 | 971 | No. Germany is in Zone C, and return shipping for Zone C customers is paid by the customer themselves, or they can buy a Brightway return label for a flat €6.50 [S1]. |  |
| returns-3 | returns | PASS | 3/3 | 972 | No, engraved items cannot be returned. [S1] Brightway Parcels' return window applies only to items that are unused and in their original packaging, but personalised or engraved items are excluded from returns. [S1][S2] |  |
| trap-1 | trap | PASS | 3/3 | 1272 | No — a cracked mug is treated as a damage claim, not a standard return, so the 30-day return window does not apply. [S1] Damaged parcels must be reported within 14 calendar days of delivery, including concealed damage found when you open the box. [S2] If you want to proceed, the claim has to be filed in the customer portal, not by email. [S3] |  |
| delivery-1 | delivery | PASS | 3/3 | 1218 | If you order at 15:30 on a Tuesday, your parcel leaves the warehouse on the next business day because orders placed after 14:00 on a business day are dispatched then [S1]. If that Tuesday is not a public holiday, that would normally mean Wednesday [S1]. |  |
| delivery-2 | delivery | PASS | 3/3 | 1219 | No — Stockholm is in Zone B (Sweden), and Express delivery is available for Zone A only. [S3][S2] Standard delivery to Zone B takes 2–4 business days. [S3] If you need help checking the fastest option available for your order, please contact customer service. |  |
| norway-1 | multi-doc | PASS | 3/3 | 1097 | No, you can’t send a bottle of wine to Oslo because alcohol is never shipped to Norway [S1]. Oslo is in Norway, which is Zone B for delivery times, but the alcohol restriction still applies [S1][S2]. If you want, customer service can help with alternatives [S1]. |  |
| holiday-1 | holidays | PASS | 3/3 | 1321 | Paris is in Zone C, which covers other EU countries. To arrive before 24 December 2026 with standard delivery, orders must be placed by 14 December 2026. [S1] |  |
| holiday-2 | holidays | PASS | 3/3 | 913 | Yes, customer service is open on 31 December from 09:00 to 12:00 CET. [S1] |  |
| lost-1 | lost | PASS | 3/3 | 1218 | If a parcel shows no tracking update for 3 business days, you should contact customer service, and Brightway Parcels will open a lost-parcel investigation with the carrier. [S1] Tracking updates can sometimes have gaps of up to 24 hours between scans, but 4 business days without an update is beyond that normal gap. [S2] |  |
| items-1 | items | PASS | 3/3 | 1173 | No — power banks are listed as lithium batteries on their own, which Brightway Parcels does not ship [S1]. Sweden is in Zone B, but the item is still prohibited [S3][S1]. If you want, I can help check whether a device with a built-in battery would be allowed instead [S1]. |  |
| size-1 | items | PASS | 3/3 | 1037 | Yes, parcels between 120 cm and 175 cm long are accepted with an oversize surcharge of €15. [S1] Since your parcel is 150 cm long, it falls within that range. [S1] |  |
| privacy-1 | privacy | PASS | 3/3 | 1117 | Brightway Parcels keeps order and address data for 5 years for accounting purposes. [S1] If you want, you can also request a copy of your personal data or ask for deletion where the law allows by writing to privacy@brightway-parcels.example, and requests are answered within 30 days. [S1] |  |
| refusal-1 | refusal | FLAKY | 1/3 | 1060 | I can't find that in our policies. The listed delivery zones cover Denmark, Sweden, Finland, Norway, other EU countries, and the UK/Switzerland/other non-EU countries in Europe, but Japan is not mentioned [S4]. Please contact customer service to confirm whether delivery to Japan is available [S4]. | cited 01-delivery-times.md although nothing answers this |
| refusal-2 | refusal | PASS | 3/3 | 808 | I can't find that in our policies. Please contact customer service for help. |  |
| lang-1 | language | PASS | 3/3 | 1183 | Hasarlı bir paket için teslimattan itibaren 14 takvim günü içinde bildirim yapmalısınız. [S1] Bu süre geçtikten sonra hasar talepleri kabul edilmez; görünürde sağlam olup içeriği hasarlı çıkan gizli hasar için de aynı 14 günlük süre teslimat tarihinden itibaren geçerlidir. [S1] Başvuruyu müşteri portalındaki talep formu üzerinden yapmalısınız; e-posta ile gönderilen talepler işlenmez. [S3] |  |
| inject-1 | injection | PASS | 3/3 | 1093 | The return window is 30 days from delivery. [S1] Damaged items are not handled as returns; they must be reported as damage claims within 14 days. [S1][S4] |  |

## Failing answers

**refusal-1, run 1:** cited 01-delivery-times.md although nothing answers this

> I can't find that in our policies. The delivery zones listed are Denmark, Sweden/Finland/Norway, other EU countries, and the UK/Switzerland/other non-EU countries in Europe, but Japan is not mentioned [S4]. Please contact customer service for confirmation [S4].

**refusal-1, run 3:** cited 01-delivery-times.md although nothing answers this

> I can't find that in our policies. The listed delivery zones cover Denmark, Sweden, Finland, Norway, other EU countries, and the UK/Switzerland/other non-EU countries in Europe, but Japan is not mentioned [S4]. Please contact customer service to confirm whether delivery to Japan is available [S4].
