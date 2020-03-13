Feature: Request session
  Scenario: Request session with no schedule returns Accepted
    When POST to sessions/request
    And content with type application/json
"""
{
  "speakers": [],
  "tags": [],
  "title": "My awesome session",
  "description": "This session is going to be awe-wait for it-some... Awesome!"
}
"""
    Then status code is Accepted
    And session store contains one entitled My awesome session
    And description is
    """
    This session is going to be awe-wait for it-some... Awesome!
    """
    And start time is
    And end time is
    And tags are
    And speakers are
    And status is Requested

  Scenario: Request session with schedule returns Accepted
    Given person 10000000-0000-0000-0000-000000000001 named John Doe
    When POST to sessions/request
    And content with type application/json
"""
{
  "speakers": ["00000000-0000-0000-0000-000000000001"],
  "tags": ["dotnet","azure"],
  "title": "My awesome session",
  "description": "This session is going to be awe-wait for it-some... Awesome!",
  "startTime": "2020-03-03T00:00:00.000",
  "endTime": "2020-03-03T01:00:00.000"
}
"""
    Then status code is Accepted
    And session store contains one entitled My awesome session
    And description is
    """
    This session is going to be awe-wait for it-some... Awesome!
    """
    And start time is 2020-03-03T00:00:00.000
    And end time is 2020-03-03T01:00:00.000
    And tags are dotnet, azure
    And speakers are 00000000-0000-0000-0000-000000000001
    And status is Requested

  Scenario: Request session with unknown speaker returns BadRequest
    When POST to sessions/request
    And content with type application/json
"""
{
  "speakers": ["12345678-0000-0000-0000-000000000001"],
  "tags": ["dotnet","azure"],
  "title": "My awesome session",
  "description": "This session is going to be awe-wait for it-some... Awesome!",
  "startTime": "2020-03-03T00:00:00.000",
  "endTime": "2020-03-03T01:00:00.000"
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Speakers are invalid: unknown speakers",
  "traceId": "*",
  "errors": {
    "Speakers": [
      "12345678-0000-0000-0000-000000000001 is unknown"
    ]
  }
}
"""
    And session store is empty

  Scenario: Request session with no startTime returns BadRequest
    When POST to sessions/request
    And content with type application/json
"""
{
  "speakers": [],
  "tags": [],
  "title": "My awesome session",
  "description": "This session is going to be awe-wait for it-some... Awesome!",
  "endTime": "2020-03-03T01:00:00.000"
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Schedule is invalid: Start time cannot be null (Parameter 'startTime').",
  "traceId": "*",
  "errors": {
    "startTime": [
      "Start time cannot be null (Parameter 'startTime')"
    ]
  }
}
"""
    And session store is empty

  Scenario: Request session with no endTime returns BadRequest
    When POST to sessions/request
    And content with type application/json
"""
{
  "speakers": [],
  "tags": [],
  "title": "My awesome session",
  "description": "This session is going to be awe-wait for it-some... Awesome!",
  "startTime": "2020-03-03T00:00:00.000"
}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Schedule is invalid: End time cannot be null (Parameter 'endTime').",
  "traceId": "*",
  "errors": {
    "endTime": [
      "End time cannot be null (Parameter 'endTime')"
    ]
  }
}
"""
    And session store is empty

  Scenario: Request session with startTime after endTime returns BadRequest
    When POST to sessions/request
    And content with type application/json
"""
{
  "speakers": [],
  "tags": [],
  "title": "My awesome session",
  "description": "This session is going to be awe-wait for it-some... Awesome!",
  "startTime": "2020-03-03T01:00:00.000",
  "endTime": "2020-03-03T00:00:00.000"
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
    And session store is empty

  Scenario: Request session with empty json returns BadRequest
    When POST to sessions/request
    And content with type application/json
"""
{}
"""
    Then status code is BadRequest
    And content matches
"""
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "*",
  "errors": {
    "Tags": [
      "The Tags field is required."
    ],
    "Title": [
      "The Title field is required."
    ],
    "Speakers": [
      "The Speakers field is required."
    ],
    "Description": [
      "The Description field is required."
    ]
  }
}
"""
    And session store is empty