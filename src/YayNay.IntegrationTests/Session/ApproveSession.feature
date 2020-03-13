Feature: Approve session
  Scenario: Approve session when admin returns Accepted
    Given a session entitled Something interesting with status Requested and id 3deb358c-d6e9-470c-bfd0-4d1499b34af9
    When POST to sessions/3deb358c-d6e9-470c-bfd0-4d1499b34af9/approval
    And authenticated as an admin
    And content with type application/json
"""
{
  "isApproved": true,
  "comment": "Cool story bro !"
}
"""
    Then status code is Accepted
    And session store contains one entitled Something interesting
    And status is Approved

  Scenario: Reject session when admin returns Accepted
    Given a session entitled Something boring with status Requested and id bb1c3492-c46d-4790-9e3d-a8dadc30a44d
    When POST to sessions/bb1c3492-c46d-4790-9e3d-a8dadc30a44d/approval
    And authenticated as an admin
    And content with type application/json
"""
{
  "isApproved": false,
  "comment": "Lol nope !"
}
"""
    Then status code is Accepted
    And session store contains one entitled Something boring
    And status is Rejected

  Scenario: Approve session when admin and session already approved returns BadRequest
    Given a session entitled Something approved with status Approved and id 236686dd-a6a4-45eb-900d-6f3794177324
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/approval
    And authenticated as an admin
    And content with type application/json
"""
{
  "isApproved": true,
  "comment": "Je m'acharne"
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Session is Approved",
  "traceId": "*"
}
"""

  Scenario: Reject session when admin and session already rejected returns BadRequest
    Given a session entitled Something approved with status Rejected and id 236686dd-a6a4-45eb-900d-6f3794177324
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/approval
    And authenticated as an admin
    And content with type application/json
"""
{
  "isApproved": false,
  "comment": "Je m'acharne"
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Session is Rejected",
  "traceId": "*"
}
"""

  Scenario: Approve session when admin and session does not exist returns NotFound
    When POST to sessions/97714acc-5383-471b-8bb2-643ee4e37874/approval
    And authenticated as an admin
    And content with type application/json
"""
{
  "isApproved": true,
  "comment": "Are you there ?"
}
"""
    Then status code is NotFound
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Session 97714acc-5383-471b-8bb2-643ee4e37874 was not found",
  "traceId": "*"
}
"""

  Scenario: Approve session when user returns Forbidden
    When POST to sessions/3deb358c-d6e9-470c-bfd0-4d1499b34af9/approval
    And authenticated as a user
    Then status code is Forbidden

  Scenario: Approve session when not authenticated Unauthorized
    When POST to sessions/3deb358c-d6e9-470c-bfd0-4d1499b34af9/approval
    Then status code is Unauthorized