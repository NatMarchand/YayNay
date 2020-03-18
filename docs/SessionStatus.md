::: mermaid
graph TB
 Requested --> Rejected
 Requested --> Approved
 Rejected --> Approved
 Approved --> Rejected
 Approved --> Scheduled
 Scheduled --> Cancelled
 Scheduled --> Archived
:::