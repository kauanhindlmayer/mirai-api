# 04 — Comment & mention notifications

**What to build:** A project member is notified when someone `@mentions` them in a work item comment or a wiki page comment, and separately, a work item's assignee is notified when anyone comments on their assigned work item - even without a mention. Mention authoring already works end-to-end in the frontend editor (TipTap mention chips); this ticket is purely about the backend consuming what's already being written and turning it into notifications, reusing ticket 01's full pipeline.

**Blocked by:** 03 — extends the same interceptor ticket 03 introduces.

**Status:** ready-for-agent

- [ ] Three new `NotificationType` values: `MentionedInWorkItemComment`, `MentionedInWikiPageComment`, `WorkItemCommentAdded`.
- [ ] The interceptor from ticket 03 is extended (not duplicated into a second interceptor) to also read newly-added `WorkItemComment`/`WikiPageComment` entries from the `ChangeTracker` each save.
- [ ] Mentioned users are identified by parsing the saved comment HTML for mention nodes (`data-type="mention" data-id="{userId}"`), creating one `MentionedInWorkItemComment` or `MentionedInWikiPageComment` notification per mentioned user, pointing at the comment/work item or wiki page.
- [ ] A `WorkItemCommentAdded` notification is created for the work item's assignee on every new comment, independent of whether they were mentioned.
- [ ] A user is never notified about their own comment (self-suppression) - whether as the assignee or as a self-mention.
- [ ] A user who is both the assignee and explicitly mentioned in the same comment receives one notification of each type (mention + comment-added), not a deduplicated single notification and not two of the same type.
- [ ] Mentions typed into a work item's Description, Acceptance Criteria, or a wiki page's body content do **not** produce notifications - only comments do. This is a deliberate v1 boundary, not a gap to fix here.
- [ ] Backend integration tests (real `AddCommentCommand`/`UpdateCommentCommand` for both work items and wiki pages, through `ISender`) cover: mention creates a notification for the mentioned user, comment on an assigned work item notifies the assignee, self-mention and self-comment are suppressed, and a user who is both mentioned and the assignee gets both notification types.
- [ ] No new frontend work is required beyond what ticket 01 already built.
