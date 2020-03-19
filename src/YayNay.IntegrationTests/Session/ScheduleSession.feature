Feature: Schedule session
  Scenario: Scheduling session with no suggested schedule returns Accepted
    Given a session entitled My awesome session with status Approved and id 236686dd-a6a4-45eb-900d-6f3794177324
    And user has right ScheduleSession
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": "2020-01-01T00:00:00",
  "endTime": "2020-01-01T01:00:00"
}
"""
    Then status code is Accepted
    And session store contains one entitled My awesome session
    And start time is 2020-01-01T00:00:00
    And end time is 2020-01-01T01:00:00
    And status is Scheduled


  Scenario: Scheduling session with suggested schedule returns Accepted
    Given a session entitled My awesome session with status Approved and id 236686dd-a6a4-45eb-900d-6f3794177324 between 2020-01-01T00:00:00 and 2020-01-01T01:00:00
    And user has right ScheduleSession
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": null,
  "endTime": null
}
"""
    Then status code is Accepted
    And session store contains one entitled My awesome session
    And start time is 2020-01-01T00:00:00
    And end time is 2020-01-01T01:00:00
    And status is Scheduled

  Scenario: Rescheduling session returns Accepted
    Given a session entitled My awesome session with status Scheduled and id 236686dd-a6a4-45eb-900d-6f3794177324 between 2020-01-01T00:00:00 and 2020-01-01T01:00:00
    And user has right ScheduleSession
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": "2020-01-02T00:00:00",
  "endTime": "2020-01-02T01:00:00"
}
"""
    Then status code is Accepted
    And session store contains one entitled My awesome session
    And start time is 2020-01-02T00:00:00
    And end time is 2020-01-02T01:00:00
    And status is Scheduled

  Scenario: Scheduling session without schedule returns BadRequest
    Given a session entitled My awesome session with status Approved and id 236686dd-a6a4-45eb-900d-6f3794177324
    And user has right ScheduleSession
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": null,
  "endTime": null
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Session has no schedule",
  "traceId": "*"
}
"""

  Scenario: Scheduling session with invalid schedule returns BadRequest
    Given a session entitled My awesome session with status Approved and id 236686dd-a6a4-45eb-900d-6f3794177324
    And user has right ScheduleSession
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": "2020-01-02T01:00:00",
  "endTime": "2020-01-02T00:00:00"
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Schedule is invalid: End time cannot be before start time (Parameter 'endTime').",
  "traceId": "*",
  "errors": {
    "endTime": [
      "End time cannot be before start time (Parameter 'endTime')"
    ]
  }
}
"""

  Scenario: Rescheduling session without schedule returns BadRequest
    Given a session entitled My awesome session with status Scheduled and id 236686dd-a6a4-45eb-900d-6f3794177324
    And user has right ScheduleSession
    When POST to sessions/236686dd-a6a4-45eb-900d-6f3794177324/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": null,
  "endTime": null
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Cannot reschedule without new schedule",
  "traceId": "*"
}
"""

  Scenario: Scheduling session when has right and session does not exist returns NotFound
    Given user has right ScheduleSession
    When POST to sessions/97714acc-5383-471b-8bb2-643ee4e37874/schedule
    And authenticated as a user
    And content with type application/json
"""
{
  "startTime": "2020-01-01T00:00:00",
  "endTime": "2020-01-01T01:00:00"
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

  Scenario: Scheduling session when no right returns Forbidden
    When POST to sessions/3deb358c-d6e9-470c-bfd0-4d1499b34af9/schedule
    And authenticated as a user
    Then status code is Forbidden

  Scenario: Scheduling session when not authenticated returns Unauthorized
    When POST to sessions/3deb358c-d6e9-470c-bfd0-4d1499b34af9/schedule
    Then status code is Unauthorized