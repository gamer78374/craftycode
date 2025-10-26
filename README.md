# CraftyCode
A rudimentary scripting language implemented in C#. 

Software contributors: Damien Otis. 

## Prerequisutes
+ Operating system: Windows
+ IDE: Visual Studio Community 2022
+ **Sandcastle Help File Builder** to build documentaion

## Structure
The solution contains *four* projects: 

### CraftyCode
A class library containing the main elements of the software. Included are classes for exceptions, parser, lexical analiser, tokeniser, a bytecode compiler and virtual machine, an operation stack. 

### CraftyCompiler
A winforms app which provides a very basic visual IDE for compiling, executing, and debugging crafty code programs. 

### SimpleTester
A command line application which some basic features to validate the proper interpretation of the grammar rules of the programming language. 

### DocCC
This is a Sandcastle project which builds the documenation for the code. Outputs to **DocCC/Help/**. 

## Language
The crafty code language is defined through text from the source code. The language is intended as a lightweight scripting language for games and other applications which require both high-data throughput with dynamic behaviour. 

### Grammar
*This is the output from the SimpleTester when the "Show Rules" command is used. The grammar is represented in a modified Chomsky normal form. TOKENs are in upper case.*

```
program
    statement_list

statement_list
    statement+

statement
    END_STATEMENT
    solid_block
    if_root_statement
    trigger_statement
    foreach_statement
    for_statement
    while_statement
    function_declare
    assign_statement END_STATEMENT
    variable_declaration_statement END_STATEMENT
    expression END_STATEMENT

variable_declaration_statement
    TYPE_IDENTIFIER IDENTIFIER assign?

assign
    assignment_operators expression

function_declare
    TYPE_IDENTIFIER IDENTIFIER BRACKET_OPEN function_argument_declaration? BRACKET_CLOSE CURLY_OPEN function_body? CURLY_CLOSE

function_body
    return_statement+
    statement+

function_argument_declaration
    TYPE_IDENTIFIER IDENTIFIER function_argument_declaration_more*

function_argument_declaration_more
    COMMA TYPE_IDENTIFIER IDENTIFIER

assign_statement
    IDENTIFIER assign

assignment_operators
    ASSIGN
    PLUS_EQUAL
    MINUS_EQUAL
    TIMES_EQUAL
    DIVIDE_EQUAL
    MOD_EQUAL

trigger_statement
    WHEN BRACKET_OPEN IDENTIFIER POINT_RIGHT IDENTIFIER.event BRACKET_CLOSE conditional_statement? CURLY_OPEN statement_list? CURLY_CLOSE

conditional_statement
    AND BRACKET_OPEN boolean_expression BRACKET_CLOSE

for_statement_third
    increment_expression
    decrement_expression
    assign_statement

for_statement
    FOR BRACKET_OPEN variable_declaration_statement? END_STATEMENT boolean_expression? END_STATEMENT for_statement_third? BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE

foreach_statement
    FOREACH BRACKET_OPEN TYPE_IDENTIFIER IDENTIFIER IN IDENTIFIER BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE

while_statement
    WHILE BRACKET_OPEN boolean_expression BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE

solid_block
    SOLID CURLY_OPEN statement_list? CURLY_CLOSE

if_root_statement
    if_statement elseif_statement* else_statement?

if_statement
    IF BRACKET_OPEN boolean_expression BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE

elseif_statement
    ELSE IF BRACKET_OPEN boolean_expression BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE

else_statement
    ELSE CURLY_OPEN statement_list? CURLY_CLOSE

return_statement
    RETURN expression? END_STATEMENT

expression
    function_call
    increment_expression
    arithmatic
    boolean_expression
    decrement_expression
    STRING

non_boolean_expression
    arithmatic
    increment_expression
    decrement_expression
    STRING

compound_expression
    BRACKET_OPEN expression BRACKET_CLOSE

function_call
    IDENTIFIER BRACKET_OPEN function_call_args? BRACKET_CLOSE

function_call_args
    expression function_call_args_more*

function_call_args_more
    COMMA expression

compound_boolean_expression
    BRACKET_OPEN boolean_expression BRACKET_CLOSE

boolean_expression
    not? boolean_atomic boolean_expression_more*

boolean_expression_more
    boolean_joiners not? boolean_atomic

boolean_atomic
    boolean_string_comparison
    boolean_float_comparison
    compound_boolean_expression
    IDENTIFIER.bool
    TRUE
    FALSE

boolean_joiners
    AND
    OR

boolean_string_comparison
    string_or_id string_comparison_operators string_or_id

boolean_float_comparison
    float_or_id float_comparison_operators float_or_id

float_or_id
    FLOAT
    function_call.float
    IDENTIFIER.float

string_or_id
    STRING
    function_call.string
    IDENTIFIER.string

float_comparison_operators
    NOT_EQUAL
    EQUALITY
    LESS_THAN
    GREATER_THAN
    GREATER_THAN_OR_EQUAL
    LESS_THAN_OR_EQUAL

string_comparison_operators
    NOT_EQUAL
    EQUALITY

compound_float_arithmatic_expression
    BRACKET_OPEN float_arithmatic_expression BRACKET_CLOSE

float_arithmatic_expression
    float_arithmatic_atomic float_arithmatic_expression_more*

float_arithmatic_expression_more
    float_arithmatic_joiners float_arithmatic_atomic

float_arithmatic_atomic
    compound_float_arithmatic_expression
    decrement_expression
    increment_expression
    IDENTIFIER.float
    FLOAT

float_arithmatic_joiners
    PLUS
    MINUS
    DIVIDE
    TIMES
    MOD

increment_expression
    IDENTIFIER.float INCREMENT
    INCREMENT IDENTIFIER.float

decrement_expression
    IDENTIFIER.float DECREMENT
    DECREMENT IDENTIFIER.float

not
    NOT

arithmatic
    float_arithmatic_expression
```
