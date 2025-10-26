# CraftyCode
A rudimentary scripting language implemented in C#. 

Software contributors: Damien Otis. 

## Technical information
Supported platforms: Windows  
Known supported IDEs: Visual Studio Community 2022

## Prerequisutes
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
    statementlist

statementlist
    statement+

statement
    ENDSTATEMENT
    solidblock
    ifrootstatement
    triggerstatement
    foreachstatement
    forstatement
    whilestatement
    functiondeclare
    assignstatement ENDSTATEMENT
    variable_declaration_statement ENDSTATEMENT
    expression ENDSTATEMENT

variable_declaration_statement
    TYPEIDENTIFIER IDENTIFIER assign?

assign
    assignoperators expression

functiondeclare
    TYPEIDENTIFIER IDENTIFIER OPENBRACKET function_argument_declaration? CLOSEBRACKET CURLYOPEN functionbody? CURLYCLOSE

functionbody
    returnstatement+
    statement+

function_argument_declaration
    TYPEIDENTIFIER IDENTIFIER function_argument_declaration_more*

function_argument_declaration_more
    COMMA TYPEIDENTIFIER IDENTIFIER

assignstatement
    IDENTIFIER assign

assignoperators
    ASSIGN
    PLUSEQUAL
    MINUSEQUAL
    TIMESEQUAL
    DIVIDEEQUAL
    MODEQUAL

triggerstatement
    WHEN OPENBRACKET IDENTIFIER POINTRIGHT IDENTIFIER.event CLOSEBRACKET conditionstatement? CURLYOPEN statementlist? CURLYCLOSE

conditionstatement
    AND OPENBRACKET booleanexpression CLOSEBRACKET

forstatement_third
    incrementexpression
    decrementexpression
    assignstatement

forstatement
    FOR OPENBRACKET variable_declaration_statement? ENDSTATEMENT booleanexpression? ENDSTATEMENT forstatement_third? CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE

foreachstatement
    FOREACH OPENBRACKET TYPEIDENTIFIER IDENTIFIER IN IDENTIFIER CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE

whilestatement
    WHILE OPENBRACKET booleanexpression CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE

solidblock
    SOLID CURLYOPEN statementlist? CURLYCLOSE

ifrootstatement
    ifstatement elseifstatement* elsestatement?

ifstatement
    IF OPENBRACKET booleanexpression CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE

elseifstatement
    ELSE IF OPENBRACKET booleanexpression CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE

elsestatement
    ELSE CURLYOPEN statementlist? CURLYCLOSE

returnstatement
    RETURN expression? ENDSTATEMENT

expression
    functioncall
    incrementexpression
    arithmatic
    booleanexpression
    decrementexpression
    STRING

non_boolean_expression
    arithmatic
    incrementexpression
    decrementexpression
    STRING

compoundexpression
    OPENBRACKET expression CLOSEBRACKET

functioncall
    IDENTIFIER OPENBRACKET function_call_args? CLOSEBRACKET

function_call_args
    expression function_call_args_more*

function_call_args_more
    COMMA expression

compoundbooleanexpression
    OPENBRACKET booleanexpression CLOSEBRACKET

booleanexpression
    not? boolean_atomic booleanexpression_more*

booleanexpression_more
    boolean_joiners not? boolean_atomic

boolean_atomic
    boolean_string_comparison
    boolean_float_comparison
    compoundbooleanexpression
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
    functioncall.float
    IDENTIFIER.float

string_or_id
    STRING
    functioncall.string
    IDENTIFIER.string

float_comparison_operators
    NOTEQUAL
    EQUITY
    LESSTHAN
    GREATERTHAN
    GREATERTHANOREQUAL
    LESSTHANOREQUAL

string_comparison_operators
    NOTEQUAL
    EQUITY

compound_float_arithmatic_expression
    OPENBRACKET float_arithmatic_expression CLOSEBRACKET

float_arithmatic_expression
    float_arithmatic_atomic float_arithmatic_expression_more*

float_arithmatic_expression_more
    float_arithmatic_joiners float_arithmatic_atomic

float_arithmatic_atomic
    compound_float_arithmatic_expression
    decrementexpression
    incrementexpression
    IDENTIFIER.float
    FLOAT

float_arithmatic_joiners
    PLUS
    MINUS
    DIVIDE
    TIMES
    MOD

incrementexpression
    IDENTIFIER.float INCREMENT
    INCREMENT IDENTIFIER.float

decrementexpression
    IDENTIFIER.float DECREMENT
    DECREMENT IDENTIFIER.float

not
    NOT

arithmatic
    float_arithmatic_expression
```
